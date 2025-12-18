namespace Deckle.API.DTOs;

/// <summary>
/// DTO for displaying an MCP access token (without the actual token value)
/// </summary>
public record McpAccessTokenDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string TokenSuffix { get; init; } // Last 4 characters for identification
    public required DateTime CreatedAt { get; init; }
    public DateTime? LastUsedAt { get; init; }
    public DateTime? RevokedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public required bool IsValid { get; init; }
}

/// <summary>
/// Request to create a new MCP access token
/// </summary>
public record CreateMcpAccessTokenRequest(
    string Name,
    string? Description,
    DateTime? ExpiresAt
);

/// <summary>
/// Response when creating a new MCP access token
/// Contains the plain token - this is the ONLY time the token is shown
/// </summary>
public record CreateMcpAccessTokenResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Token { get; init; } // Plain token - only shown once!
    public required string TokenSuffix { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}

/// <summary>
/// Request to update token metadata (name, description)
/// </summary>
public record UpdateMcpAccessTokenRequest(
    string Name,
    string? Description
);
