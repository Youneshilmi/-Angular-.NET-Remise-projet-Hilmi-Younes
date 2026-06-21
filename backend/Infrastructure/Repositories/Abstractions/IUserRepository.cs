using Lensrock.Core.Entities;

namespace Lensrock.Infrastructure.Repositories.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<int> CreateAsync(User user);
}
