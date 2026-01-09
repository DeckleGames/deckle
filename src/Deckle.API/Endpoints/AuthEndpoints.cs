using Deckle.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace Deckle.API.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth")
            .WithTags("Authentication");

        group.MapGet("/login", (IConfiguration configuration, ILogger<Program> logger, HttpContext context) =>
        {
            var frontendUrl = configuration["FrontendUrl"];

            // In production, FrontendUrl must be configured
            if (string.IsNullOrWhiteSpace(frontendUrl))
            {
                var isDevelopment = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
                if (isDevelopment)
                {
                    frontendUrl = "http://localhost:5173";
                    logger.LogWarning("FrontendUrl not configured, using default: {FrontendUrl}", frontendUrl);
                }
                else
                {
                    logger.LogError("FrontendUrl is not configured in production environment");
                    return Results.Problem("FrontendUrl is not configured", statusCode: 500);
                }
            }

            // Validate that frontendUrl is an absolute URL
            if (!Uri.TryCreate(frontendUrl, UriKind.Absolute, out var uri))
            {
                logger.LogError("Invalid FrontendUrl configuration: {FrontendUrl}", frontendUrl);
                return Results.Problem("Invalid FrontendUrl configuration", statusCode: 500);
            }

            var redirectUri = $"{frontendUrl.TrimEnd('/')}/projects";
            logger.LogInformation("Auth login initiated. Redirecting to: {RedirectUri}", redirectUri);

            return Results.Challenge(
                new AuthenticationProperties { RedirectUri = redirectUri },
                new[] { GoogleDefaults.AuthenticationScheme }
            );
        })
        .AllowAnonymous()
        .WithName("Login");

        group.MapPost("/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Ok(new { message = "Logged out successfully" });
        })
        .RequireAuthorization()
        .WithName("Logout");

        group.MapGet("/me", (ClaimsPrincipal user) =>
        {
            var currentUser = UserService.GetCurrentUserFromClaims(user);

            if (currentUser == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(currentUser);
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser");

        return group;
    }
}
