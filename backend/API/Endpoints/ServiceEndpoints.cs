using Lensrock.Core.Entities;
using Lensrock.Core.Models;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.API.Endpoints;

public static class ServiceEndpoints
{
    public static IEndpointRouteBuilder MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/services");

        group.MapGet("", async (
            string? search,
            int? categoryId,
            ICatalogUseCases catalog) =>
            Results.Ok(await catalog.GetAllAsync(search, categoryId)))
            .AllowAnonymous();

        group.MapGet("/admin", async (ICatalogUseCases catalog) =>
            Results.Ok(await catalog.GetAllAsync(null, null, true)))
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapGet("/categories", async (ICatalogUseCases catalog) =>
            Results.Ok(await catalog.GetCategoriesAsync()))
            .AllowAnonymous();

        group.MapGet("/{id:int}", async (int id, ICatalogUseCases catalog) =>
            Results.Ok(await catalog.GetByIdAsync(id)))
            .AllowAnonymous();

        group.MapPost("", async (ServiceInput input, ICatalogUseCases catalog) =>
        {
            var id = await catalog.CreateAsync(input);
            return Results.Created($"/api/services/{id}", new { id });
        }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapPut("/{id:int}", async (int id, ServiceInput input, ICatalogUseCases catalog) =>
        {
            await catalog.UpdateAsync(id, input);
            return Results.NoContent();
        }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapDelete("/{id:int}", async (int id, ICatalogUseCases catalog) =>
        {
            await catalog.DeleteAsync(id);
            return Results.NoContent();
        }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        return app;
    }
}
