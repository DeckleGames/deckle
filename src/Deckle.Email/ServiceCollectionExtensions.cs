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
    /// Adds email sending services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration to bind EmailOptions from.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind configuration
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

        // Register email sender
        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }

    /// <summary>
    /// Adds email sending services with explicit options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure email options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        Action<EmailOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddScoped<IEmailSender, EmailSender>();
        return services;
    }
}
