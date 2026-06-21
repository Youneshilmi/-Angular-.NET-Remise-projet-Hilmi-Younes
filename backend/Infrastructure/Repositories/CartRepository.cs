using Dapper;
using Lensrock.Core.Entities;
using Lensrock.Infrastructure.Database;
using Lensrock.Infrastructure.Repositories.Abstractions;

namespace Lensrock.Infrastructure.Repositories;

public sealed class CartRepository(SqlConnectionFactory connections) : ICartRepository
{
    public async Task<IReadOnlyCollection<CartItem>> GetByUserAsync(int userId)
    {
        const string sql = """
            SELECT ci.Id, ci.UserId, ci.ServiceId, s.Name AS ServiceName,
                   s.DurationMinutes, s.Price AS UnitPrice, ci.Quantity
            FROM CartItems ci
            INNER JOIN Services s ON s.Id = ci.ServiceId
            WHERE ci.UserId = @UserId AND s.IsActive = 1
            ORDER BY ci.Id DESC;
            """;
        await using var connection = connections.Create();
        return (await connection.QueryAsync<CartItem>(sql, new { UserId = userId })).AsList();
    }

    public async Task UpsertAsync(int userId, int serviceId, int quantity)
    {
        const string sql = """
            IF EXISTS (SELECT 1 FROM CartItems WHERE UserId = @UserId AND ServiceId = @ServiceId)
                UPDATE CartItems
                SET Quantity = CASE WHEN Quantity + @Quantity > 10 THEN 10 ELSE Quantity + @Quantity END
                WHERE UserId = @UserId AND ServiceId = @ServiceId;
            ELSE
                INSERT INTO CartItems (UserId, ServiceId, Quantity)
                VALUES (@UserId, @ServiceId, @Quantity);
            """;
        await using var connection = connections.Create();
        await connection.ExecuteAsync(sql, new { UserId = userId, ServiceId = serviceId, Quantity = quantity });
    }

    public async Task<bool> UpdateQuantityAsync(int userId, int serviceId, int quantity)
    {
        const string sql = """
            UPDATE CartItems SET Quantity = @Quantity
            WHERE UserId = @UserId AND ServiceId = @ServiceId;
            """;
        await using var connection = connections.Create();
        return await connection.ExecuteAsync(sql, new
        {
            UserId = userId,
            ServiceId = serviceId,
            Quantity = quantity
        }) > 0;
    }

    public async Task<bool> RemoveAsync(int userId, int serviceId)
    {
        const string sql = "DELETE FROM CartItems WHERE UserId = @UserId AND ServiceId = @ServiceId;";
        await using var connection = connections.Create();
        return await connection.ExecuteAsync(sql, new { UserId = userId, ServiceId = serviceId }) > 0;
    }
}
