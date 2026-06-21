using Lensrock.Core.Entities;
using Lensrock.Core.IGateways;
using Lensrock.Infrastructure.Repositories.Abstractions;

namespace Lensrock.Infrastructure.Gateways;

public sealed class OrderGateway(IOrderRepository orders) : IOrderGateway
{
    public Task<int> CreateFromCartAsync(
        int userId,
        DateTime bookingDate,
        string notes,
        IReadOnlyCollection<CartItem> items) => orders.CreateFromCartAsync(userId, bookingDate, notes, items);

    public Task<IReadOnlyCollection<Order>> GetByUserAsync(int userId) => orders.GetByUserAsync(userId);
    public Task<IReadOnlyCollection<Order>> GetAllAsync() => orders.GetAllAsync();
    public Task<bool> UpdateStatusAsync(int orderId, string status) => orders.UpdateStatusAsync(orderId, status);
}
