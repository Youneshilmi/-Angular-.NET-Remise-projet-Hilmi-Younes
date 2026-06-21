using Microsoft.Extensions.DependencyInjection;
using Lensrock.Core.UseCases;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.Core;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthUseCases, AuthUseCases>();
        services.AddScoped<ICatalogUseCases, CatalogUseCases>();
        services.AddScoped<ICartUseCases, CartUseCases>();
        services.AddScoped<IOrderUseCases, OrderUseCases>();
        services.AddScoped<IAdminUseCases, AdminUseCases>();
        return services;
    }
}
