using Dapper;
using Lensrock.Core.Entities;
using Lensrock.Infrastructure.Database;
using Lensrock.Infrastructure.Repositories.Abstractions;

namespace Lensrock.Infrastructure.Repositories;

public sealed class UserRepository(SqlConnectionFactory connections) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = """
            SELECT Id, FullName, Email, PasswordHash, Role, CreatedAt
            FROM Users
            WHERE Email = @Email;
            """;
        await using var connection = connections.Create();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT Id, FullName, Email, PasswordHash, Role, CreatedAt
            FROM Users
            WHERE Id = @Id;
            """;
        await using var connection = connections.Create();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(User user)
    {
        const string sql = """
            INSERT INTO Users (FullName, Email, PasswordHash, Role)
            VALUES (@FullName, @Email, @PasswordHash, @Role);
            SELECT CAST(SCOPE_IDENTITY() AS int);
            """;
        await using var connection = connections.Create();
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }
}
