using Dapper;
using Lensrock.Core.Models;
using Lensrock.Infrastructure.Database;
using Lensrock.Infrastructure.Repositories.Abstractions;

namespace Lensrock.Infrastructure.Repositories;

public sealed class AdminRepository(SqlConnectionFactory connections) : IAdminRepository
{
    public async Task<DashboardSummary> GetDashboardAsync()
    {
        const string sql = """
            SELECT
                (SELECT COUNT(*) FROM Users) AS UserCount,
                (SELECT COUNT(*) FROM Services WHERE IsActive = 1) AS ActiveServiceCount,
                (SELECT COUNT(*) FROM Orders) AS OrderCount,
                (SELECT COALESCE(SUM(TotalAmount), 0) FROM Orders WHERE Status <> 'Cancelled') AS Revenue;
            """;
        await using var connection = connections.Create();
        return await connection.QuerySingleAsync<DashboardSummary>(sql);
    }
}
