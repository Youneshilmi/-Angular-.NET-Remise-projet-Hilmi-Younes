using Lensrock.Core.Entities;

namespace Lensrock.Infrastructure.Repositories.Abstractions;

public interface IOrderRepository
{
    Task<int> CreateFromCartAsync(int userId, DateTime bookingDate, string notes, IReadOnlyCollection<CartItem> items);
    Task<IReadOnlyCollection<Order>> GetByUserAsync(int userId);
    Task<IReadOnlyCollection<Order>> GetAllAsync();
    Task<bool> UpdateStatusAsync(int orderId, string status);
}
