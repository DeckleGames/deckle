using Deckle.API.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Deckle.API.Endpoints;

public static class GoogleSheetsAuthEndpoints
{
    public static RouteGroupBuilder MapGoogleSheetsAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/google-sheets-auth")
            .WithTags("GoogleSheetsAuth");

        // Check if user has authorized Google Sheets
        group.MapGet("status", async (ClaimsPrincipal user, UserService userService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var hasAuth = await userService.HasGoogleSheetsAuthAsync(userId.Value);
            return Results.Ok(new { authorized = hasAuth });
        })
        .RequireAuthorization()
        .WithName("GetGoogleSheetsAuthStatus");

        // Initiate Google Sheets OAuth flow
        group.MapGet("authorize", (HttpContext httpContext, string? returnUrl) =>
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = $"/google-sheets-auth/callback?returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}"
            };

            // Force consent to ensure we get a refresh token
            properties.Items["prompt"] = "consent";

            // Add custom scopes - include profile and email so Google OAuth handler can fetch user info
            // The handler requires these to complete successfully
            properties.Items["scope"] = "profile email https://www.googleapis.com/auth/spreadsheets.readonly https://www.googleapis.com/auth/drive.readonly";

            // Use the main Google scheme but with custom scopes
            return Results.Challenge(properties, new[] { "Google" });
        })
        .RequireAuthorization()
        .WithName("AuthorizeGoogleSheets");

        // OAuth callback handler
        group.MapGet("callback", async (HttpContext httpContext, string? returnUrl, UserService userService) =>
        {
            var userId = UserService.GetUserIdFromClaims(httpContext.User);

            if (userId == null)
            {
                return Results.Redirect($"{returnUrl ?? "/"}?error=unauthorized");
            }

            // Get the authentication result from the main Google scheme
            var authenticateResult = await httpContext.AuthenticateAsync("Google");

            if (!authenticateResult.Succeeded)
            {
                return Results.Redirect($"{returnUrl ?? "/"}?error=auth_failed");
            }

            // Extract tokens
            var tokens = authenticateResult.Properties?.Items;
            var accessToken = tokens?.FirstOrDefault(x => x.Key == ".Token.access_token").Value;
            var refreshToken = tokens?.FirstOrDefault(x => x.Key == ".Token.refresh_token").Value;
            var tokenType = tokens?.FirstOrDefault(x => x.Key == ".Token.token_type").Value ?? "Bearer";
            var expiresAt = tokens?.FirstOrDefault(x => x.Key == ".Token.expires_at").Value;
            var scope = tokens?.FirstOrDefault(x => x.Key == ".Token.scope").Value ?? "";

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                return Results.Redirect($"{returnUrl ?? "/"}?error=no_tokens");
            }

            // Parse expiration
            DateTime expiresAtDate = DateTime.UtcNow.AddHours(1);
            if (!string.IsNullOrEmpty(expiresAt) && DateTime.TryParse(expiresAt, out var parsedDate))
            {
                expiresAtDate = parsedDate.ToUniversalTime();
            }

            // Save the credentials
            await userService.SaveOrUpdateGoogleCredentialAsync(
                userId.Value,
                accessToken,
                refreshToken,
                tokenType,
                expiresAtDate,
                scope
            );

            return Results.Redirect($"{returnUrl ?? "/"}?success=true");
        })
        .RequireAuthorization()
        .WithName("GoogleSheetsCallback");

        // Revoke Google Sheets authorization
        group.MapPost("revoke", async (ClaimsPrincipal user, UserService userService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            await userService.RevokeGoogleCredentialAsync(userId.Value);
            return Results.Ok(new { success = true });
        })
        .RequireAuthorization()
        .WithName("RevokeGoogleSheetsAuth");

        return group;
    }
}
