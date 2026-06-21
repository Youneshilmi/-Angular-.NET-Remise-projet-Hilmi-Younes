using Lensrock.Core.Entities;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.API.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapGet("/dashboard", async (IAdminUseCases admin) =>
            Results.Ok(await admin.GetDashboardAsync()));

        return app;
    }
}
