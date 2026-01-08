using System.Reflection;

namespace Deckle.API.Endpoints;

public static class VersionEndpoints
{
    public static RouteGroupBuilder MapVersionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api")
            .WithTags("Version");

        group.MapGet("/version", () =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            var informationalVersion = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;
            var fileVersion = assembly
                .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                .Version;

            return Results.Ok(new
            {
                version = version?.ToString() ?? "0.0.0.0",
                informationalVersion = informationalVersion ?? "0.0.0",
                fileVersion = fileVersion ?? "0.0.0.0",
                buildDate = GetBuildDate(assembly),
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            });
        })
        .AllowAnonymous()
        .WithName("GetVersion")
        .WithDescription("Get application version information");

        return group;
    }

    private static DateTime GetBuildDate(Assembly assembly)
    {
        // Use the last write time of the assembly file as a proxy for build date
        var location = assembly.Location;
        if (string.IsNullOrEmpty(location))
        {
            return DateTime.UtcNow;
        }

        try
        {
            return File.GetLastWriteTimeUtc(location);
        }
        catch
        {
            return DateTime.UtcNow;
        }
    }
}
