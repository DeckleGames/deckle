namespace Deckle.Email.Abstractions;

/// <summary>
/// Base interface for all email templates.
/// Each email template should implement this interface and provide
/// the necessary data to generate and send an email.
/// </summary>
public interface IEmailTemplate
{
    /// <summary>
    /// The recipient(s) of the email.
    /// </summary>
    IReadOnlyList<EmailAddress> To { get; }

    /// <summary>
    /// The email subject line.
    /// </summary>
    string Subject { get; }

    /// <summary>
    /// The HTML body of the email.
    /// </summary>
    string HtmlBody { get; }

    /// <summary>
    /// The plain text body of the email (optional, but recommended for accessibility).
    /// If not provided, a plain text version may be auto-generated from HTML.
    /// </summary>
    string? TextBody => null;

    /// <summary>
    /// Optional CC recipients.
    /// </summary>
    IReadOnlyList<EmailAddress>? Cc => null;

    /// <summary>
    /// Optional BCC recipients.
    /// </summary>
    IReadOnlyList<EmailAddress>? Bcc => null;

    /// <summary>
    /// Optional reply-to address (if different from sender).
    /// </summary>
    EmailAddress? ReplyTo => null;

    /// <summary>
    /// Email priority (defaults to Normal).
    /// </summary>
    EmailPriority Priority => EmailPriority.Normal;

    /// <summary>
    /// Optional attachments.
    /// </summary>
    IReadOnlyList<EmailAttachment>? Attachments => null;
}
