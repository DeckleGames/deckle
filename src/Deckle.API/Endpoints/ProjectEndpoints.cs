using Deckle.API.DTOs;
using Deckle.API.Services;
using System.Security.Claims;

namespace Deckle.API.Endpoints;

public static class ProjectEndpoints
{
    public static RouteGroupBuilder MapProjectEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/projects")
            .WithTags("Projects")
            .RequireAuthorization();

        group.MapGet("", async (ClaimsPrincipal user, ProjectService projectService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var projects = await projectService.GetUserProjectsAsync(userId.Value);
            return Results.Ok(projects);
        })
        .WithName("GetProjects");

        group.MapGet("{id:guid}", async (Guid id, ClaimsPrincipal user, ProjectService projectService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var project = await projectService.GetProjectByIdAsync(userId.Value, id);

            if (project == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(project);
        })
        .WithName("GetProjectById");

        group.MapPost("", async (ClaimsPrincipal user, ProjectService projectService, CreateProjectRequest request) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var project = await projectService.CreateProjectAsync(userId.Value, request.Name, request.Description);
            return Results.Created($"/projects/{project.Id}", project);
        })
        .WithName("CreateProject");

        group.MapPut("{id:guid}", async (Guid id, ClaimsPrincipal user, ProjectService projectService, UpdateProjectRequest request) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var project = await projectService.UpdateProjectAsync(userId.Value, id, request.Name, request.Description);

            if (project == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(project);
        })
        .WithName("UpdateProject");

        group.MapGet("{id:guid}/users", async (Guid id, ClaimsPrincipal user, ProjectService projectService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var users = await projectService.GetProjectUsersAsync(userId.Value, id);
            return Results.Ok(users);
        })
        .WithName("GetProjectUsers");

        group.MapDelete("{id:guid}", async (Guid id, ClaimsPrincipal user, ProjectService projectService) =>
        {
            var userId = UserService.GetUserIdFromClaims(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var success = await projectService.DeleteProjectAsync(userId.Value, id);

            if (!success)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .WithName("DeleteProject");

        return group;
    }
}
