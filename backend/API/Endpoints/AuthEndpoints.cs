using Lensrock.API.Security;
using Lensrock.Core.Entities;
using Lensrock.Core.Models;
using Lensrock.Core.UseCases.Abstractions;

namespace Lensrock.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").AllowAnonymous();

        group.MapPost("/register", async (
            RegisterRequest request,
            IAuthUseCases auth,
            JwtTokenService tokens) =>
        {
            var user = await auth.RegisterAsync(request);
            return Results.Ok(ToResponse(user, tokens));
        });

        group.MapPost("/login", async (
            LoginRequest request,
            IAuthUseCases auth,
            JwtTokenService tokens) =>
        {
            var user = await auth.LoginAsync(request);
            return Results.Ok(ToResponse(user, tokens));
        });

        return app;
    }

    private static object ToResponse(User user, JwtTokenService tokens) => new
    {
        token = tokens.Create(user),
        user = new { user.Id, user.FullName, user.Email, user.Role }
    };
}
