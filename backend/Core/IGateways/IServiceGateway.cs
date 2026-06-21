using Lensrock.Core.Entities;

namespace Lensrock.Core.IGateways;

public interface IServiceGateway
{
    Task<IReadOnlyCollection<Category>> GetCategoriesAsync();
    Task<IReadOnlyCollection<ServiceOffering>> GetAllAsync(string? search, int? categoryId, bool includeInactive);
    Task<ServiceOffering?> GetByIdAsync(int id, bool includeInactive = false);
    Task<int> CreateAsync(ServiceOffering service);
    Task<bool> UpdateAsync(ServiceOffering service);
    Task<bool> DeleteAsync(int id);
}
