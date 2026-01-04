namespace Deckle.Email.Abstractions;

/// <summary>
/// Represents an email address with an optional display name.
/// </summary>
public record EmailAddress
{
    /// <summary>
    /// The email address (e.g., "user@example.com").
    /// </summary>
    public required string Address { get; init; }

    /// <summary>
    /// Optional display name (e.g., "John Smith").
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Creates an EmailAddress from just an address string.
    /// </summary>
    public static EmailAddress From(string address) => new() { Address = address };

    /// <summary>
    /// Creates an EmailAddress with both address and display name.
    /// </summary>
    public static EmailAddress From(string address, string displayName) =>
        new() { Address = address, DisplayName = displayName };
}
