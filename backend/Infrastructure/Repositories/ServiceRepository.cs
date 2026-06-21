using Dapper;
using Lensrock.Core.Entities;
using Lensrock.Infrastructure.Database;
using Lensrock.Infrastructure.Repositories.Abstractions;

namespace Lensrock.Infrastructure.Repositories;

public sealed class ServiceRepository(SqlConnectionFactory connections) : IServiceRepository
{
    public async Task<IReadOnlyCollection<Category>> GetCategoriesAsync()
    {
        const string sql = "SELECT Id, Name FROM Categories ORDER BY Name;";
        await using var connection = connections.Create();
        return (await connection.QueryAsync<Category>(sql)).AsList();
    }

    public async Task<IReadOnlyCollection<ServiceOffering>> GetAllAsync(
        string? search,
        int? categoryId,
        bool includeInactive)
    {
        const string sql = """
            SELECT s.Id, s.CategoryId, c.Name AS CategoryName, s.Name, s.Description,
                   s.DurationMinutes, s.Price, s.ImageUrl, s.IsActive
            FROM Services s
            INNER JOIN Categories c ON c.Id = s.CategoryId
            WHERE (@IncludeInactive = 1 OR s.IsActive = 1)
              AND (@CategoryId IS NULL OR s.CategoryId = @CategoryId)
              AND (@Search IS NULL OR s.Name LIKE '%' + @Search + '%'
                   OR s.Description LIKE '%' + @Search + '%')
            ORDER BY s.Name;
            """;
        await using var connection = connections.Create();
        var normalizedSearch = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return (await connection.QueryAsync<ServiceOffering>(sql, new
        {
            Search = normalizedSearch,
            CategoryId = categoryId,
            IncludeInactive = includeInactive
        })).AsList();
    }

    public async Task<ServiceOffering?> GetByIdAsync(int id, bool includeInactive = false)
    {
        const string sql = """
            SELECT s.Id, s.CategoryId, c.Name AS CategoryName, s.Name, s.Description,
                   s.DurationMinutes, s.Price, s.ImageUrl, s.IsActive
            FROM Services s
            INNER JOIN Categories c ON c.Id = s.CategoryId
            WHERE s.Id = @Id AND (@IncludeInactive = 1 OR s.IsActive = 1);
            """;
        await using var connection = connections.Create();
        return await connection.QuerySingleOrDefaultAsync<ServiceOffering>(sql, new
        {
            Id = id,
            IncludeInactive = includeInactive
        });
    }

    public async Task<int> CreateAsync(ServiceOffering service)
    {
        const string sql = """
            INSERT INTO Services
                (CategoryId, Name, Description, DurationMinutes, Price, ImageUrl, IsActive)
            VALUES
                (@CategoryId, @Name, @Description, @DurationMinutes, @Price, @ImageUrl, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() AS int);
            """;
        await using var connection = connections.Create();
        return await connection.ExecuteScalarAsync<int>(sql, service);
    }

    public async Task<bool> UpdateAsync(ServiceOffering service)
    {
        const string sql = """
            UPDATE Services
            SET CategoryId = @CategoryId,
                Name = @Name,
                Description = @Description,
                DurationMinutes = @DurationMinutes,
                Price = @Price,
                ImageUrl = @ImageUrl,
                IsActive = @IsActive
            WHERE Id = @Id;
            """;
        await using var connection = connections.Create();
        return await connection.ExecuteAsync(sql, service) > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "UPDATE Services SET IsActive = 0 WHERE Id = @Id;";
        await using var connection = connections.Create();
        return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
