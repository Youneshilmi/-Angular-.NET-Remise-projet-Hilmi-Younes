using Lensrock.Core.Entities;
using Lensrock.Core.IGateways;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.Core.UseCases;

public sealed class CartUseCases(ICartGateway cart, IServiceGateway services) : ICartUseCases
{
    public Task<IReadOnlyCollection<CartItem>> GetAsync(int userId) => cart.GetByUserAsync(userId);

    public async Task AddAsync(int userId, int serviceId, int quantity)
    {
        if (quantity is < 1 or > 10)
        {
            throw new ArgumentException("La quantité doit être comprise entre 1 et 10.");
        }

        _ = await services.GetByIdAsync(serviceId)
            ?? throw new KeyNotFoundException("Service introuvable ou indisponible.");
        await cart.UpsertAsync(userId, serviceId, quantity);
    }

    public async Task UpdateAsync(int userId, int serviceId, int quantity)
    {
        if (quantity is < 1 or > 10)
        {
            throw new ArgumentException("La quantité doit être comprise entre 1 et 10.");
        }

        if (!await cart.UpdateQuantityAsync(userId, serviceId, quantity))
        {
            throw new KeyNotFoundException("Article introuvable dans le panier.");
        }
    }

    public async Task RemoveAsync(int userId, int serviceId)
    {
        if (!await cart.RemoveAsync(userId, serviceId))
        {
            throw new KeyNotFoundException("Article introuvable dans le panier.");
        }
    }
}
