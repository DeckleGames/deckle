var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL with pgAdmin and persistent data volume
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Configure pgAdmin with health check
postgres.WithPgAdmin(configureContainer: container => container
    .WithHttpHealthCheck("/browser/"));

var database = postgres.AddDatabase("deckledb");

var web = builder.AddNpmApp("web", "../Deckle.Web")
    .WithHttpEndpoint(port: 5173, env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

var api = builder.AddProject<Projects.Deckle_API>("api")
    .WithReference(database)
    .WithReference(web)
    .WithEnvironment("FrontendUrl", web.GetEndpoint("http"))
    .WaitFor(database);

web.WithReference(api)
    .WithEnvironment("PUBLIC_API_URL", api.GetEndpoint("http"));

builder.Build().Run();
