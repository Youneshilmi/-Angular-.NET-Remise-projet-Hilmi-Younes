using Lensrock.Core.IGateways;
using Lensrock.Core.Models;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.Core.UseCases;

public sealed class AdminUseCases(IAdminGateway admin) : IAdminUseCases
{
    public Task<DashboardSummary> GetDashboardAsync() => admin.GetDashboardAsync();
}
