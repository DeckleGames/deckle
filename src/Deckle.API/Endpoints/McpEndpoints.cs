using Deckle.API.DTOs;
using Deckle.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Deckle.API.Endpoints;

public static class McpEndpoints
{
    public static RouteGroupBuilder MapMcpEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/mcp")
            .WithTags("MCP Server");

        // MCP message endpoint (handles all MCP protocol messages)
        group.MapPost("", async (
            HttpContext httpContext,
            McpService mcpService,
            McpAccessTokenService tokenService,
            [FromBody] McpRequest request) =>
        {
            // Authenticate using bearer token
            var userId = await AuthenticateMcpRequest(httpContext, tokenService);
            if (userId == null)
            {
                return Results.Json(new McpResponse
                {
                    Id = request.Id,
                    Error = new McpError
                    {
                        Code = -32000,
                        Message = "Authentication required. Please provide a valid MCP access token in the Authorization header."
                    }
                }, statusCode: 401);
            }

            try
            {
                // Handle different MCP methods
                return request.Method switch
                {
                    "initialize" => Results.Json(new McpResponse
                    {
                        Id = request.Id,
                        Result = mcpService.GetInitializeResult()
                    }),

                    "tools/list" => Results.Json(new McpResponse
                    {
                        Id = request.Id,
                        Result = mcpService.GetToolsList()
                    }),

                    "tools/call" => await HandleToolCall(request, userId.Value, mcpService),

                    _ => Results.Json(new McpResponse
                    {
                        Id = request.Id,
                        Error = new McpError
                        {
                            Code = -32601,
                            Message = $"Method not found: {request.Method}"
                        }
                    }, statusCode: 404)
                };
            }
            catch (Exception ex)
            {
                return Results.Json(new McpResponse
                {
                    Id = request.Id,
                    Error = new McpError
                    {
                        Code = -32603,
                        Message = "Internal error",
                        Data = ex.Message
                    }
                }, statusCode: 500);
            }
        })
        .WithName("McpMessage")
        .WithDescription("MCP protocol endpoint for AI agent interaction");

        return group;
    }

    /// <summary>
    /// Authenticate MCP request using bearer token
    /// </summary>
    private static async Task<Guid?> AuthenticateMcpRequest(
        HttpContext httpContext,
        McpAccessTokenService tokenService)
    {
        // Get authorization header
        var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader))
        {
            return null;
        }

        // Extract bearer token
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var token = authHeader.Substring(7);
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        // Verify token and get user ID
        return await tokenService.VerifyTokenAsync(token);
    }

    /// <summary>
    /// Handle tool call requests
    /// </summary>
    private static async Task<IResult> HandleToolCall(
        McpRequest request,
        Guid userId,
        McpService mcpService)
    {
        try
        {
            // Parse tool call parameters
            var paramsJson = JsonSerializer.Serialize(request.Params);
            var toolCallParams = JsonSerializer.Deserialize<McpToolCallParams>(paramsJson);

            if (toolCallParams == null)
            {
                return Results.Json(new McpResponse
                {
                    Id = request.Id,
                    Error = new McpError
                    {
                        Code = -32602,
                        Message = "Invalid params for tools/call"
                    }
                }, statusCode: 400);
            }

            // Execute the tool
            var result = await mcpService.ExecuteToolAsync(
                userId,
                toolCallParams.Name,
                toolCallParams.Arguments);

            return Results.Json(new McpResponse
            {
                Id = request.Id,
                Result = result
            });
        }
        catch (Exception ex)
        {
            return Results.Json(new McpResponse
            {
                Id = request.Id,
                Error = new McpError
                {
                    Code = -32603,
                    Message = "Error executing tool",
                    Data = ex.Message
                }
            }, statusCode: 500);
        }
    }
}
