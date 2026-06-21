using Lensrock.Core.Entities;
using Lensrock.Core.IGateways;
using Lensrock.Infrastructure.Repositories.Abstractions;

namespace Lensrock.Infrastructure.Gateways;

public sealed class CartGateway(ICartRepository cart) : ICartGateway
{
    public Task<IReadOnlyCollection<CartItem>> GetByUserAsync(int userId) => cart.GetByUserAsync(userId);
    public Task UpsertAsync(int userId, int serviceId, int quantity) => cart.UpsertAsync(userId, serviceId, quantity);
    public Task<bool> UpdateQuantityAsync(int userId, int serviceId, int quantity) =>
        cart.UpdateQuantityAsync(userId, serviceId, quantity);
    public Task<bool> RemoveAsync(int userId, int serviceId) => cart.RemoveAsync(userId, serviceId);
}
