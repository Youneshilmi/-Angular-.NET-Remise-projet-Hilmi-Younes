using Lensrock.Core.IGateways;
using Lensrock.Core.Models;
using Lensrock.Infrastructure.Repositories.Abstractions;

namespace Lensrock.Infrastructure.Gateways;

public sealed class AdminGateway(IAdminRepository admin) : IAdminGateway
{
    public Task<DashboardSummary> GetDashboardAsync() => admin.GetDashboardAsync();
}
