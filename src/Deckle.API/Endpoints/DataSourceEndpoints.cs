using Deckle.API.Services;
using System.Security.Claims;

namespace Deckle.API.Endpoints;

public static class DataSourceEndpoints
{
    public static RouteGroupBuilder MapDataSourceEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/data-sources")
            .WithTags("DataSources")
            .RequireAuthorization();

        group.MapGet("project/{projectId:guid}", async (Guid projectId, ClaimsPrincipal user, DataSourceService dataSourceService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            try
            {
                var dataSources = await dataSourceService.GetProjectDataSourcesAsync(userId.Value, projectId);
                return Results.Ok(dataSources);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
        })
        .WithName("GetProjectDataSources");

        group.MapGet("{id:guid}", async (Guid id, ClaimsPrincipal user, DataSourceService dataSourceService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            try
            {
                var dataSource = await dataSourceService.GetDataSourceByIdAsync(userId.Value, id);

                if (dataSource == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(dataSource);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
        })
        .WithName("GetDataSourceById");

        group.MapPost("", async (ClaimsPrincipal user, DataSourceService dataSourceService, CreateDataSourceRequest request) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            try
            {
                var dataSource = await dataSourceService.CreateGoogleSheetsDataSourceAsync(
                    userId.Value,
                    request.ProjectId,
                    request.Name,
                    request.Url
                );

                return Results.Created($"/data-sources/{dataSource.Id}", dataSource);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("CreateDataSource");

        group.MapDelete("{id:guid}", async (Guid id, ClaimsPrincipal user, DataSourceService dataSourceService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            try
            {
                var deleted = await dataSourceService.DeleteDataSourceAsync(userId.Value, id);

                if (!deleted)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
        })
        .WithName("DeleteDataSource");

        // Endpoint to get spreadsheet metadata
        group.MapGet("{id:guid}/metadata", async (Guid id, ClaimsPrincipal user, DataSourceService dataSourceService, GoogleSheetsService googleSheetsService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            try
            {
                var dataSource = await dataSourceService.GetDataSourceByIdAsync(userId.Value, id);

                if (dataSource == null)
                {
                    return Results.NotFound();
                }

                if (string.IsNullOrEmpty(dataSource.GoogleSheetsId))
                {
                    return Results.BadRequest(new { error = "Data source is not a Google Sheet" });
                }

                var metadata = await googleSheetsService.GetSpreadsheetMetadataAsync(userId.Value, dataSource.GoogleSheetsId);
                return Results.Ok(metadata);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
        })
        .WithName("GetDataSourceMetadata");

        // Endpoint to get sheet data
        group.MapGet("{id:guid}/data", async (Guid id, string? range, ClaimsPrincipal user, DataSourceService dataSourceService, GoogleSheetsService googleSheetsService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            try
            {
                var dataSource = await dataSourceService.GetDataSourceByIdAsync(userId.Value, id);

                if (dataSource == null)
                {
                    return Results.NotFound();
                }

                if (string.IsNullOrEmpty(dataSource.GoogleSheetsId))
                {
                    return Results.BadRequest(new { error = "Data source is not a Google Sheet" });
                }

                // Default to first sheet if no range specified
                var dataRange = range ?? "A1:Z1000";
                var data = await googleSheetsService.GetSheetDataAsync(userId.Value, dataSource.GoogleSheetsId, dataRange);

                return Results.Ok(new { data });
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
        })
        .WithName("GetDataSourceData");

        return group;
    }
}
