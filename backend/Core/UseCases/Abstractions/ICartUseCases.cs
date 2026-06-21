using Lensrock.Core.Entities;

namespace Lensrock.Core.UseCases.Abstractions;

public interface ICartUseCases
{
    Task<IReadOnlyCollection<CartItem>> GetAsync(int userId);
    Task AddAsync(int userId, int serviceId, int quantity);
    Task UpdateAsync(int userId, int serviceId, int quantity);
    Task RemoveAsync(int userId, int serviceId);
}
