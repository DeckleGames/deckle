namespace Deckle.Email;

/// <summary>
/// Configuration options for email sending.
/// </summary>
public class EmailOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Email";

    /// <summary>
    /// SMTP server host (e.g., "smtp.gmail.com").
    /// </summary>
    public required string SmtpHost { get; init; }

    /// <summary>
    /// SMTP server port (typically 587 for TLS, 465 for SSL, 25 for non-encrypted).
    /// </summary>
    public required int SmtpPort { get; init; }

    /// <summary>
    /// Whether to use SSL/TLS encryption.
    /// </summary>
    public bool UseSsl { get; init; } = true;

    /// <summary>
    /// SMTP username (often the email address).
    /// </summary>
    public required string? Username { get; init; }

    /// <summary>
    /// SMTP password or app-specific password.
    /// </summary>
    public required string? Password { get; init; }

    /// <summary>
    /// Default "from" address for outgoing emails.
    /// </summary>
    public required string FromAddress { get; init; }

    /// <summary>
    /// Default "from" display name.
    /// </summary>
    public string FromName { get; init; } = "Deckle";

    /// <summary>
    /// Timeout in seconds for SMTP operations.
    /// </summary>
    public int TimeoutSeconds { get; init; } = 30;
}
