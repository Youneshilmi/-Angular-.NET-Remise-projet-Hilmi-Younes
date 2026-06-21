using Lensrock.Core.Entities;
using Lensrock.Core.IGateways;
using Lensrock.Infrastructure.Repositories.Abstractions;

namespace Lensrock.Infrastructure.Gateways;

public sealed class UserGateway(IUserRepository users) : IUserGateway
{
    public Task<User?> GetByEmailAsync(string email) => users.GetByEmailAsync(email);
    public Task<User?> GetByIdAsync(int id) => users.GetByIdAsync(id);
    public Task<int> CreateAsync(User user) => users.CreateAsync(user);
}
