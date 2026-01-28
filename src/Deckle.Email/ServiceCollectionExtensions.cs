using Deckle.Email.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deckle.Email;

/// <summary>
/// Extension methods for registering email services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds SMTP-based email sending services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration to bind EmailOptions from.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSmtpEmailServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        return services;
    }

    /// <summary>
    /// Adds SMTP-based email sending services with explicit options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure email options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSmtpEmailServices(
        this IServiceCollection services,
        Action<EmailOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        return services;
    }

    /// <summary>
    /// Adds Brevo-based email sending services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration to bind BrevoOptions from.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddBrevoEmailServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<BrevoOptions>(configuration.GetSection(BrevoOptions.SectionName));
        services.AddHttpClient<IEmailSender, BrevoEmailSender>();
        return services;
    }

    /// <summary>
    /// Adds Brevo-based email sending services with explicit options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure Brevo options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddBrevoEmailServices(
        this IServiceCollection services,
        Action<BrevoOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddHttpClient<IEmailSender, BrevoEmailSender>();
        return services;
    }

    /// <summary>
    /// Adds email sending services based on the "Email:Provider" configuration value.
    /// Supported providers: "Smtp" (default), "Brevo".
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration.GetSection("Email")["Provider"];

        if (string.Equals(provider, "Brevo", StringComparison.OrdinalIgnoreCase))
        {
            return services.AddBrevoEmailServices(configuration);
        }

        return services.AddSmtpEmailServices(configuration);
    }
}
