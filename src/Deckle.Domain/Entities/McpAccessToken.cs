namespace Deckle.Domain.Entities;

/// <summary>
/// Represents an MCP (Model Context Protocol) access token for API authentication.
/// Tokens are hashed before storage for security. The plain token is only shown once during creation.
/// </summary>
public class McpAccessToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    /// <summary>
    /// User-friendly name for the token (e.g., "Production Server", "Development Bot")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// SHA256 hash of the token. The plain token is never stored.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Last 4 characters of the plain token for identification in UI
    /// </summary>
    public string TokenSuffix { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of what this token is used for
    /// </summary>
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Optional expiration date. If null, token never expires.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    public User User { get; set; } = null!;

    /// <summary>
    /// Check if the token is currently valid (not revoked and not expired)
    /// </summary>
    public bool IsValid => RevokedAt == null && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);
}
