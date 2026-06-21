using Lensrock.Core.Entities;

namespace Lensrock.Core.IGateways;

public interface IUserGateway
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<int> CreateAsync(User user);
}
