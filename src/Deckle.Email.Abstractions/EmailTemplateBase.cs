namespace Deckle.Email.Abstractions;

/// <summary>
/// Base class for email templates providing common functionality.
/// Inherit from this class to create email templates more easily.
/// </summary>
public abstract class EmailTemplateBase : IEmailTemplate
{
    public abstract IReadOnlyList<EmailAddress> To { get; }
    public abstract string Subject { get; }
    public abstract string HtmlBody { get; }
    public virtual string? TextBody => null;
    public virtual IReadOnlyList<EmailAddress>? Cc => null;
    public virtual IReadOnlyList<EmailAddress>? Bcc => null;
    public virtual EmailAddress? ReplyTo => null;
    public virtual EmailPriority Priority => EmailPriority.Normal;
    public virtual IReadOnlyList<EmailAttachment>? Attachments => null;

    /// <summary>
    /// Helper method to wrap content in a responsive email layout with proper styling.
    /// </summary>
    protected static string WrapInEmailLayout(string title, string bodyContent)
    {
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <title>{title}</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            line-height: 1.6;
            color: #2c3e50;
            background-color: #f8f9fa;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07);
        }}
        .email-header {{
            background-color: #78a083;
            color: #ffffff;
            padding: 30px 40px;
            text-align: center;
            border-bottom: 1px solid rgba(255, 255, 255, 0.15);
        }}
        .email-header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
            letter-spacing: 0.02em;
        }}
        .email-body {{
            padding: 40px;
            color: #2c3e50;
        }}
        .email-footer {{
            background-color: #f8f9fa;
            padding: 30px 40px;
            text-align: center;
            font-size: 14px;
            color: #5a6c7d;
            border-top: 1px solid rgba(52, 73, 86, 0.12);
        }}
        .button {{
            display: inline-block;
            padding: 12px 24px;
            background-color: #78a083;
            color: #ffffff !important;
            text-decoration: none;
            border-radius: 8px;
            font-weight: 500;
            margin: 20px 0;
            box-shadow: 0 2px 4px rgba(120, 160, 131, 0.2);
            transition: background-color 0.2s ease;
        }}
        .button:hover {{
            background-color: #6a8f75;
        }}
        h2 {{
            color: #2c3e50;
            font-size: 20px;
            margin-top: 0;
            font-weight: 600;
        }}
        p {{
            margin: 16px 0;
            color: #2c3e50;
        }}
        ul, ol {{
            color: #2c3e50;
        }}
        strong {{
            color: #344956;
        }}
        .highlight {{
            background-color: rgba(120, 160, 131, 0.15);
            color: #344956;
            padding: 2px 8px;
            border-radius: 4px;
            font-weight: 500;
        }}
    </style>
</head>
<body>
    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"">
        <tr>
            <td style=""padding: 20px 0;"">
                <table role=""presentation"" class=""email-container"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                    <tr>
                        <td class=""email-header"">
                            <h1>Deckle</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class=""email-body"">
                            {bodyContent}
                        </td>
                    </tr>
                    <tr>
                        <td class=""email-footer"">
                            <p>This email was sent by Deckle</p>
                            <p>&copy; {DateTime.UtcNow.Year} Deckle. All rights reserved.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
