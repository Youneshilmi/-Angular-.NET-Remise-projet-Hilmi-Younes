using Lensrock.Core.Entities;
using Lensrock.Core.IGateways;
using Lensrock.Core.Models;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.Core.UseCases;

public sealed class CatalogUseCases(IServiceGateway services) : ICatalogUseCases
{
    public Task<IReadOnlyCollection<Category>> GetCategoriesAsync() => services.GetCategoriesAsync();

    public Task<IReadOnlyCollection<ServiceOffering>> GetAllAsync(
        string? search,
        int? categoryId,
        bool includeInactive = false) => services.GetAllAsync(search, categoryId, includeInactive);

    public async Task<ServiceOffering> GetByIdAsync(int id, bool includeInactive = false) =>
        await services.GetByIdAsync(id, includeInactive)
        ?? throw new KeyNotFoundException("Service introuvable.");

    public async Task<int> CreateAsync(ServiceInput input)
    {
        Validate(input);
        return await services.CreateAsync(ToEntity(input));
    }

    public async Task UpdateAsync(int id, ServiceInput input)
    {
        Validate(input);
        var service = ToEntity(input);
        service.Id = id;
        if (!await services.UpdateAsync(service))
        {
            throw new KeyNotFoundException("Service introuvable.");
        }
    }

    public async Task DeleteAsync(int id)
    {
        if (!await services.DeleteAsync(id))
        {
            throw new KeyNotFoundException("Service introuvable.");
        }
    }

    private static ServiceOffering ToEntity(ServiceInput input) => new()
    {
        CategoryId = input.CategoryId,
        Name = input.Name.Trim(),
        Description = input.Description.Trim(),
        DurationMinutes = input.DurationMinutes,
        Price = input.Price,
        ImageUrl = input.ImageUrl.Trim(),
        IsActive = input.IsActive
    };

    private static void Validate(ServiceInput input)
    {
        if (input.CategoryId <= 0 || string.IsNullOrWhiteSpace(input.Name))
        {
            throw new ArgumentException("La catégorie et le nom sont obligatoires.");
        }
        if (input.DurationMinutes <= 0 || input.Price < 0)
        {
            throw new ArgumentException("La durée et le prix doivent être valides.");
        }
    }
}
