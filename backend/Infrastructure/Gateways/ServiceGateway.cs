using Lensrock.Core.Entities;
using Lensrock.Core.IGateways;
using Lensrock.Infrastructure.Repositories.Abstractions;

namespace Lensrock.Infrastructure.Gateways;

public sealed class ServiceGateway(IServiceRepository services) : IServiceGateway
{
    public Task<IReadOnlyCollection<Category>> GetCategoriesAsync() => services.GetCategoriesAsync();
    public Task<IReadOnlyCollection<ServiceOffering>> GetAllAsync(string? search, int? categoryId, bool includeInactive) =>
        services.GetAllAsync(search, categoryId, includeInactive);
    public Task<ServiceOffering?> GetByIdAsync(int id, bool includeInactive = false) =>
        services.GetByIdAsync(id, includeInactive);
    public Task<int> CreateAsync(ServiceOffering service) => services.CreateAsync(service);
    public Task<bool> UpdateAsync(ServiceOffering service) => services.UpdateAsync(service);
    public Task<bool> DeleteAsync(int id) => services.DeleteAsync(id);
}
