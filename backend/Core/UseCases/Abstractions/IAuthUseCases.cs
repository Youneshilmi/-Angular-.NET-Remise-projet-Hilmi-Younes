using Lensrock.Core.Entities;
using Lensrock.Core.Models;

namespace Lensrock.Core.UseCases.Abstractions;

public interface IAuthUseCases
{
    Task<User> RegisterAsync(RegisterRequest request);
    Task<User> LoginAsync(LoginRequest request);
}
