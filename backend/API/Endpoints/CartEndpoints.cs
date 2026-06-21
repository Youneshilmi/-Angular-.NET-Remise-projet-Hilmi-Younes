using Lensrock.API.Security;
using Lensrock.Core.Models;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.API.Endpoints;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cart").RequireAuthorization();

        group.MapGet("", async (HttpContext context, ICartUseCases cart) =>
            Results.Ok(await cart.GetAsync(context.User.GetUserId())));

        group.MapPost("", async (
            HttpContext context,
            AddCartItemRequest request,
            ICartUseCases cart) =>
        {
            await cart.AddAsync(context.User.GetUserId(), request.ServiceId, request.Quantity);
            return Results.NoContent();
        });

        group.MapPut("/{serviceId:int}", async (
            HttpContext context,
            int serviceId,
            UpdateCartItemRequest request,
            ICartUseCases cart) =>
        {
            await cart.UpdateAsync(context.User.GetUserId(), serviceId, request.Quantity);
            return Results.NoContent();
        });

        group.MapDelete("/{serviceId:int}", async (
            HttpContext context,
            int serviceId,
            ICartUseCases cart) =>
        {
            await cart.RemoveAsync(context.User.GetUserId(), serviceId);
            return Results.NoContent();
        });

        return app;
    }
}
