namespace Deckle.Email.Abstractions;

/// <summary>
/// Represents an email attachment.
/// </summary>
public record EmailAttachment
{
    /// <summary>
    /// The filename of the attachment (e.g., "document.pdf").
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// The content of the attachment as a byte array.
    /// </summary>
    public required byte[] Content { get; init; }

    /// <summary>
    /// The MIME type of the attachment (e.g., "application/pdf").
    /// </summary>
    public required string ContentType { get; init; }
}
