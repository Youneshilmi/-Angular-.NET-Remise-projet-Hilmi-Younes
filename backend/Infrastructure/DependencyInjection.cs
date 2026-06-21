using Microsoft.Extensions.DependencyInjection;
using Lensrock.Core.IGateways;
using Lensrock.Core.Interfaces;
using Lensrock.Infrastructure.Database;
using Lensrock.Infrastructure.Gateways;
using Lensrock.Infrastructure.Repositories;
using Lensrock.Infrastructure.Repositories.Abstractions;
using Lensrock.Infrastructure.Security;

namespace Lensrock.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new SqlConnectionFactory(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IUserGateway, UserGateway>();
        services.AddScoped<IServiceGateway, ServiceGateway>();
        services.AddScoped<ICartGateway, CartGateway>();
        services.AddScoped<IOrderGateway, OrderGateway>();
        services.AddScoped<IAdminGateway, AdminGateway>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        return services;
    }
}
