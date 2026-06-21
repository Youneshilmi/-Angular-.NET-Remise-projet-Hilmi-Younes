using Lensrock.Core.Models;

namespace Lensrock.Core.UseCases.Abstractions;

public interface IAdminUseCases
{
    Task<DashboardSummary> GetDashboardAsync();
}
