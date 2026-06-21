using Lensrock.Core.Entities;
using Lensrock.Core.IGateways;
using Lensrock.Core.Models;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.Core.UseCases;

public sealed class OrderUseCases(IOrderGateway orders, ICartGateway cart) : IOrderUseCases
{
    public async Task<int> CheckoutAsync(int userId, CheckoutRequest request)
    {
        if (request.BookingDate < DateTime.Today.AddDays(1))
        {
            throw new ArgumentException("La réservation doit être prévue au minimum pour demain.");
        }

        var items = await cart.GetByUserAsync(userId);
        if (items.Count == 0)
        {
            throw new InvalidOperationException("Le panier est vide.");
        }

        return await orders.CreateFromCartAsync(
            userId,
            request.BookingDate,
            request.Notes.Trim(),
            items);
    }

    public Task<IReadOnlyCollection<Order>> GetMineAsync(int userId) => orders.GetByUserAsync(userId);
    public Task<IReadOnlyCollection<Order>> GetAllAsync() => orders.GetAllAsync();

    public async Task UpdateStatusAsync(int orderId, string status)
    {
        if (!OrderStatuses.All.Contains(status))
        {
            throw new ArgumentException("Statut de commande invalide.");
        }

        if (!await orders.UpdateStatusAsync(orderId, status))
        {
            throw new KeyNotFoundException("Commande introuvable.");
        }
    }
}
