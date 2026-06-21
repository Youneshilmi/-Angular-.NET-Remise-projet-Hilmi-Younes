using Lensrock.Core.Entities;

namespace Lensrock.Core.IGateways;

public interface IOrderGateway
{
    Task<int> CreateFromCartAsync(int userId, DateTime bookingDate, string notes, IReadOnlyCollection<CartItem> items);
    Task<IReadOnlyCollection<Order>> GetByUserAsync(int userId);
    Task<IReadOnlyCollection<Order>> GetAllAsync();
    Task<bool> UpdateStatusAsync(int orderId, string status);
}
