using Deckle.API.DTOs;
using Deckle.API.Filters;
using Deckle.API.Services;

namespace Deckle.API.Endpoints;

public static class McpAccessTokenEndpoints
{
    public static RouteGroupBuilder MapMcpAccessTokenEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/mcp-tokens")
            .WithTags("MCP Access Tokens")
            .RequireAuthorization()
            .RequireUserId(); // Apply user ID validation to all endpoints in this group

        // List all tokens for the authenticated user
        group.MapGet("", async (HttpContext httpContext, McpAccessTokenService tokenService) =>
        {
            var userId = httpContext.GetUserId();
            var tokens = await tokenService.ListTokensAsync(userId);
            return Results.Ok(tokens);
        })
        .WithName("ListMcpTokens")
        .WithDescription("List all MCP access tokens for the authenticated user");

        // Get a single token by ID
        group.MapGet("{id:guid}", async (Guid id, HttpContext httpContext, McpAccessTokenService tokenService) =>
        {
            var userId = httpContext.GetUserId();
            var token = await tokenService.GetTokenAsync(userId, id);

            if (token == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(token);
        })
        .WithName("GetMcpToken")
        .WithDescription("Get a specific MCP access token by ID");

        // Create a new token
        group.MapPost("", async (
            HttpContext httpContext,
            McpAccessTokenService tokenService,
            CreateMcpAccessTokenRequest request) =>
        {
            var userId = httpContext.GetUserId();

            // Validate request
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest(new { error = "Token name is required" });
            }

            if (request.Name.Length > 255)
            {
                return Results.BadRequest(new { error = "Token name must be 255 characters or less" });
            }

            if (request.Description?.Length > 1000)
            {
                return Results.BadRequest(new { error = "Token description must be 1000 characters or less" });
            }

            // Check if expiration date is in the past
            if (request.ExpiresAt.HasValue && request.ExpiresAt.Value <= DateTime.UtcNow)
            {
                return Results.BadRequest(new { error = "Expiration date must be in the future" });
            }

            var token = await tokenService.CreateTokenAsync(
                userId,
                request.Name,
                request.Description,
                request.ExpiresAt);

            return Results.Created($"/mcp-tokens/{token.Id}", token);
        })
        .WithName("CreateMcpToken")
        .WithDescription("Create a new MCP access token. The plain token is only returned once!");

        // Update token metadata (name, description)
        group.MapPut("{id:guid}", async (
            Guid id,
            HttpContext httpContext,
            McpAccessTokenService tokenService,
            UpdateMcpAccessTokenRequest request) =>
        {
            var userId = httpContext.GetUserId();

            // Validate request
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest(new { error = "Token name is required" });
            }

            if (request.Name.Length > 255)
            {
                return Results.BadRequest(new { error = "Token name must be 255 characters or less" });
            }

            if (request.Description?.Length > 1000)
            {
                return Results.BadRequest(new { error = "Token description must be 1000 characters or less" });
            }

            var success = await tokenService.UpdateTokenAsync(userId, id, request.Name, request.Description);

            if (!success)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .WithName("UpdateMcpToken")
        .WithDescription("Update MCP access token metadata (name and description)");

        // Revoke a token (soft delete)
        group.MapPost("{id:guid}/revoke", async (Guid id, HttpContext httpContext, McpAccessTokenService tokenService) =>
        {
            var userId = httpContext.GetUserId();
            var success = await tokenService.RevokeTokenAsync(userId, id);

            if (!success)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .WithName("RevokeMcpToken")
        .WithDescription("Revoke an MCP access token (soft delete - sets RevokedAt timestamp)");

        // Delete a token permanently (hard delete)
        group.MapDelete("{id:guid}", async (Guid id, HttpContext httpContext, McpAccessTokenService tokenService) =>
        {
            var userId = httpContext.GetUserId();
            var success = await tokenService.DeleteTokenAsync(userId, id);

            if (!success)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .WithName("DeleteMcpToken")
        .WithDescription("Permanently delete an MCP access token (hard delete)");

        return group;
    }
}
