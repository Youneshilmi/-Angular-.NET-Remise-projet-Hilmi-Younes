using Lensrock.Core.Models;

namespace Lensrock.Core.IGateways;

public interface IAdminGateway
{
    Task<DashboardSummary> GetDashboardAsync();
}
