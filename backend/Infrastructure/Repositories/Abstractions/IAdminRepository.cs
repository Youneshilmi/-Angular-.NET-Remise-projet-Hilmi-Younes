using Lensrock.Core.Models;

namespace Lensrock.Infrastructure.Repositories.Abstractions;

public interface IAdminRepository
{
    Task<DashboardSummary> GetDashboardAsync();
}
