using Lensrock.Core.Entities;
using Lensrock.Core.IGateways;
using Lensrock.Core.Interfaces;
using Lensrock.Core.Models;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.Core.UseCases;

public sealed class AuthUseCases(IUserGateway users, IPasswordHasher passwordHasher) : IAuthUseCases
{
    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        var fullName = request.FullName.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        if (fullName.Length < 2)
        {
            throw new ArgumentException("Le nom complet doit contenir au moins 2 caractères.");
        }

        if (!email.Contains('@'))
        {
            throw new ArgumentException("L'adresse e-mail est invalide.");
        }

        if (request.Password.Length < 8)
        {
            throw new ArgumentException("Le mot de passe doit contenir au moins 8 caractères.");
        }

        if (await users.GetByEmailAsync(email) is not null)
        {
            throw new InvalidOperationException("Un compte existe déjà avec cette adresse e-mail.");
        }

        var user = new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = Roles.Customer
        };
        user.Id = await users.CreateAsync(user);
        return user;
    }

    public async Task<User> LoginAsync(LoginRequest request)
    {
        var user = await users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("E-mail ou mot de passe incorrect.");
        }

        return user;
    }
}
