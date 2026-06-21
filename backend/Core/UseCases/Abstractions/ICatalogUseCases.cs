using Lensrock.Core.Entities;
using Lensrock.Core.Models;

namespace Lensrock.Core.UseCases.Abstractions;

public interface ICatalogUseCases
{
    Task<IReadOnlyCollection<Category>> GetCategoriesAsync();
    Task<IReadOnlyCollection<ServiceOffering>> GetAllAsync(string? search, int? categoryId, bool includeInactive = false);
    Task<ServiceOffering> GetByIdAsync(int id, bool includeInactive = false);
    Task<int> CreateAsync(ServiceInput input);
    Task UpdateAsync(int id, ServiceInput input);
    Task DeleteAsync(int id);
}
