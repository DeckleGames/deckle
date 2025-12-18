using Deckle.API.DTOs;
using Deckle.Domain.Data;
using Deckle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Deckle.API.Services;

public class McpAccessTokenService
{
    private readonly AppDbContext _dbContext;
    private const int TokenLength = 32; // 32 bytes = 256 bits

    public McpAccessTokenService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Generate a cryptographically secure random token
    /// </summary>
    private static string GenerateSecureToken()
    {
        var tokenBytes = new byte[TokenLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);

        // Convert to base64url (URL-safe base64)
        var token = Convert.ToBase64String(tokenBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");

        return $"mcp_{token}"; // Prefix for easy identification
    }

    /// <summary>
    /// Hash a token using SHA256
    /// </summary>
    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// Get the last 4 characters of a token for identification
    /// </summary>
    private static string GetTokenSuffix(string token)
    {
        return token.Length >= 4 ? token.Substring(token.Length - 4) : token;
    }

    /// <summary>
    /// Create a new MCP access token for a user
    /// </summary>
    public async Task<CreateMcpAccessTokenResponse> CreateTokenAsync(
        Guid userId,
        string name,
        string? description,
        DateTime? expiresAt)
    {
        var plainToken = GenerateSecureToken();
        var tokenHash = HashToken(plainToken);
        var tokenSuffix = GetTokenSuffix(plainToken);

        var token = new McpAccessToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            TokenHash = tokenHash,
            TokenSuffix = tokenSuffix,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        _dbContext.McpAccessTokens.Add(token);
        await _dbContext.SaveChangesAsync();

        return new CreateMcpAccessTokenResponse
        {
            Id = token.Id,
            Name = token.Name,
            Description = token.Description,
            Token = plainToken, // Only time the plain token is returned!
            TokenSuffix = token.TokenSuffix,
            CreatedAt = token.CreatedAt,
            ExpiresAt = token.ExpiresAt
        };
    }

    /// <summary>
    /// List all tokens for a user (active and revoked)
    /// </summary>
    public async Task<List<McpAccessTokenDto>> ListTokensAsync(Guid userId)
    {
        return await _dbContext.McpAccessTokens
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new McpAccessTokenDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                TokenSuffix = t.TokenSuffix,
                CreatedAt = t.CreatedAt,
                LastUsedAt = t.LastUsedAt,
                RevokedAt = t.RevokedAt,
                ExpiresAt = t.ExpiresAt,
                IsValid = t.RevokedAt == null && (t.ExpiresAt == null || t.ExpiresAt > DateTime.UtcNow)
            })
            .ToListAsync();
    }

    /// <summary>
    /// Get a single token by ID for a user
    /// </summary>
    public async Task<McpAccessTokenDto?> GetTokenAsync(Guid userId, Guid tokenId)
    {
        return await _dbContext.McpAccessTokens
            .Where(t => t.UserId == userId && t.Id == tokenId)
            .Select(t => new McpAccessTokenDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                TokenSuffix = t.TokenSuffix,
                CreatedAt = t.CreatedAt,
                LastUsedAt = t.LastUsedAt,
                RevokedAt = t.RevokedAt,
                ExpiresAt = t.ExpiresAt,
                IsValid = t.RevokedAt == null && (t.ExpiresAt == null || t.ExpiresAt > DateTime.UtcNow)
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Revoke a token (soft delete - sets RevokedAt timestamp)
    /// </summary>
    public async Task<bool> RevokeTokenAsync(Guid userId, Guid tokenId)
    {
        var token = await _dbContext.McpAccessTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == tokenId);

        if (token == null)
        {
            return false;
        }

        token.RevokedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Update token metadata (name and description)
    /// </summary>
    public async Task<bool> UpdateTokenAsync(
        Guid userId,
        Guid tokenId,
        string name,
        string? description)
    {
        var token = await _dbContext.McpAccessTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == tokenId);

        if (token == null)
        {
            return false;
        }

        token.Name = name;
        token.Description = description;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Verify a token and return the associated user ID if valid
    /// This method should be called by authentication middleware
    /// </summary>
    public async Task<Guid?> VerifyTokenAsync(string plainToken)
    {
        var tokenHash = HashToken(plainToken);

        var token = await _dbContext.McpAccessTokens
            .Where(t => t.TokenHash == tokenHash)
            .FirstOrDefaultAsync();

        if (token == null)
        {
            return null; // Token not found
        }

        // Check if token is valid (not revoked and not expired)
        if (token.RevokedAt != null)
        {
            return null; // Token has been revoked
        }

        if (token.ExpiresAt.HasValue && token.ExpiresAt.Value <= DateTime.UtcNow)
        {
            return null; // Token has expired
        }

        // Update last used timestamp
        token.LastUsedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return token.UserId;
    }

    /// <summary>
    /// Delete a token permanently (hard delete)
    /// </summary>
    public async Task<bool> DeleteTokenAsync(Guid userId, Guid tokenId)
    {
        var token = await _dbContext.McpAccessTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == tokenId);

        if (token == null)
        {
            return false;
        }

        _dbContext.McpAccessTokens.Remove(token);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
