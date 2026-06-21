using Dapper;
using Lensrock.Core.Entities;
using Lensrock.Infrastructure.Database;
using Lensrock.Infrastructure.Repositories.Abstractions;

namespace Lensrock.Infrastructure.Repositories;

public sealed class OrderRepository(SqlConnectionFactory connections) : IOrderRepository
{
    public async Task<int> CreateFromCartAsync(
        int userId,
        DateTime bookingDate,
        string notes,
        IReadOnlyCollection<CartItem> items)
    {
        const string orderSql = """
            INSERT INTO Orders (UserId, BookingDate, Notes, Status, TotalAmount)
            VALUES (@UserId, @BookingDate, @Notes, @Status, @TotalAmount);
            SELECT CAST(SCOPE_IDENTITY() AS int);
            """;
        const string itemSql = """
            INSERT INTO OrderItems
                (OrderId, ServiceId, ServiceName, DurationMinutes, UnitPrice, Quantity)
            VALUES
                (@OrderId, @ServiceId, @ServiceName, @DurationMinutes, @UnitPrice, @Quantity);
            """;
        const string clearCartSql = "DELETE FROM CartItems WHERE UserId = @UserId;";

        await using var connection = connections.Create();
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var orderId = await connection.ExecuteScalarAsync<int>(orderSql, new
            {
                UserId = userId,
                BookingDate = bookingDate,
                Notes = notes,
                Status = OrderStatuses.Pending,
                TotalAmount = items.Sum(item => item.LineTotal)
            }, transaction);

            foreach (var item in items)
            {
                await connection.ExecuteAsync(itemSql, new
                {
                    OrderId = orderId,
                    item.ServiceId,
                    item.ServiceName,
                    item.DurationMinutes,
                    item.UnitPrice,
                    item.Quantity
                }, transaction);
            }

            await connection.ExecuteAsync(clearCartSql, new { UserId = userId }, transaction);
            await transaction.CommitAsync();
            return orderId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public Task<IReadOnlyCollection<Order>> GetByUserAsync(int userId) => GetOrdersAsync(userId);
    public Task<IReadOnlyCollection<Order>> GetAllAsync() => GetOrdersAsync(null);

    public async Task<bool> UpdateStatusAsync(int orderId, string status)
    {
        const string sql = "UPDATE Orders SET Status = @Status WHERE Id = @Id;";
        await using var connection = connections.Create();
        return await connection.ExecuteAsync(sql, new { Id = orderId, Status = status }) > 0;
    }

    private async Task<IReadOnlyCollection<Order>> GetOrdersAsync(int? userId)
    {
        const string orderSql = """
            SELECT o.Id, o.UserId, u.FullName AS CustomerName, u.Email AS CustomerEmail,
                   o.BookingDate, o.Notes, o.Status, o.TotalAmount, o.CreatedAt
            FROM Orders o
            INNER JOIN Users u ON u.Id = o.UserId
            WHERE @UserId IS NULL OR o.UserId = @UserId
            ORDER BY o.CreatedAt DESC;
            """;
        const string itemSql = """
            SELECT oi.Id, oi.OrderId, oi.ServiceId, oi.ServiceName,
                   oi.DurationMinutes, oi.UnitPrice, oi.Quantity
            FROM OrderItems oi
            INNER JOIN Orders o ON o.Id = oi.OrderId
            WHERE @UserId IS NULL OR o.UserId = @UserId
            ORDER BY oi.Id;
            """;

        await using var connection = connections.Create();
        var orders = (await connection.QueryAsync<Order>(orderSql, new { UserId = userId })).AsList();
        var items = (await connection.QueryAsync<OrderItem>(itemSql, new { UserId = userId })).AsList();
        var itemsByOrder = items.GroupBy(item => item.OrderId).ToDictionary(group => group.Key, group => group.ToArray());
        foreach (var order in orders)
        {
            order.Items = itemsByOrder.GetValueOrDefault(order.Id) ?? [];
        }
        return orders;
    }
}
