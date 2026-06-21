using Lensrock.Core.Entities;

namespace Lensrock.Infrastructure.Repositories.Abstractions;

public interface ICartRepository
{
    Task<IReadOnlyCollection<CartItem>> GetByUserAsync(int userId);
    Task UpsertAsync(int userId, int serviceId, int quantity);
    Task<bool> UpdateQuantityAsync(int userId, int serviceId, int quantity);
    Task<bool> RemoveAsync(int userId, int serviceId);
}
