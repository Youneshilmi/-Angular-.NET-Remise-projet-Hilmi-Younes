using Lensrock.Core.Entities;
using Lensrock.Core.Models;

namespace Lensrock.Core.UseCases.Abstractions;

public interface IOrderUseCases
{
    Task<int> CheckoutAsync(int userId, CheckoutRequest request);
    Task<IReadOnlyCollection<Order>> GetMineAsync(int userId);
    Task<IReadOnlyCollection<Order>> GetAllAsync();
    Task UpdateStatusAsync(int orderId, string status);
}
