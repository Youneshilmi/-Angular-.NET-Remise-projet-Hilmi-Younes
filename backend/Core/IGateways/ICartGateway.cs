using Lensrock.Core.Entities;

namespace Lensrock.Core.IGateways;

public interface ICartGateway
{
    Task<IReadOnlyCollection<CartItem>> GetByUserAsync(int userId);
    Task UpsertAsync(int userId, int serviceId, int quantity);
    Task<bool> UpdateQuantityAsync(int userId, int serviceId, int quantity);
    Task<bool> RemoveAsync(int userId, int serviceId);
}
