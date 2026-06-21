using Lensrock.API.Security;
using Lensrock.Core.Entities;
using Lensrock.Core.Models;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.API.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").RequireAuthorization();

        group.MapPost("/checkout", async (
            HttpContext context,
            CheckoutRequest request,
            IOrderUseCases orders) =>
        {
            var id = await orders.CheckoutAsync(context.User.GetUserId(), request);
            return Results.Ok(new { id });
        });

        group.MapGet("/mine", async (HttpContext context, IOrderUseCases orders) =>
            Results.Ok(await orders.GetMineAsync(context.User.GetUserId())));

        group.MapGet("/admin", async (IOrderUseCases orders) =>
            Results.Ok(await orders.GetAllAsync()))
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapPatch("/{id:int}/status", async (
            int id,
            UpdateOrderStatusRequest request,
            IOrderUseCases orders) =>
        {
            await orders.UpdateStatusAsync(id, request.Status);
            return Results.NoContent();
        }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        return app;
    }
}
