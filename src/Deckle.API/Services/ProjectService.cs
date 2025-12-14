using Deckle.API.DTOs;
using Deckle.Domain.Data;
using Deckle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Deckle.API.Services;

public class ProjectService
{
    private readonly AppDbContext _dbContext;

    public ProjectService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ProjectDto>> GetUserProjectsAsync(Guid userId)
    {
        var projects = await _dbContext.UserProjects
            .Where(up => up.UserId == userId)
            .Include(up => up.Project)
            .Select(up => new ProjectDto
            {
                Id = up.Project.Id,
                Name = up.Project.Name,
                Description = up.Project.Description,
                CreatedAt = up.Project.CreatedAt,
                UpdatedAt = up.Project.UpdatedAt,
                Role = up.Role.ToString()
            })
            .ToListAsync();

        return projects;
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid userId, Guid projectId)
    {
        var project = await _dbContext.UserProjects
            .Where(up => up.UserId == userId && up.ProjectId == projectId)
            .Include(up => up.Project)
            .Select(up => new ProjectDto
            {
                Id = up.Project.Id,
                Name = up.Project.Name,
                Description = up.Project.Description,
                CreatedAt = up.Project.CreatedAt,
                UpdatedAt = up.Project.UpdatedAt,
                Role = up.Role.ToString()
            })
            .FirstOrDefaultAsync();

        return project;
    }

    public async Task<ProjectDto> CreateProjectAsync(Guid userId, string name, string? description)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Projects.Add(project);

        var userProject = new UserProject
        {
            UserId = userId,
            ProjectId = project.Id,
            Role = ProjectRole.Owner,
            JoinedAt = DateTime.UtcNow
        };

        _dbContext.UserProjects.Add(userProject);

        await _dbContext.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            Role = userProject.Role.ToString()
        };
    }

    public async Task<ProjectDto?> UpdateProjectAsync(Guid userId, Guid projectId, string name, string? description)
    {
        var userProject = await _dbContext.UserProjects
            .Where(up => up.UserId == userId && up.ProjectId == projectId)
            .Include(up => up.Project)
            .FirstOrDefaultAsync();

        if (userProject == null)
        {
            return null;
        }

        // Only Owner and Admin can update project details
        if (userProject.Role != ProjectRole.Owner && userProject.Role != ProjectRole.Admin)
        {
            return null;
        }

        userProject.Project.Name = name;
        userProject.Project.Description = description;
        userProject.Project.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new ProjectDto
        {
            Id = userProject.Project.Id,
            Name = userProject.Project.Name,
            Description = userProject.Project.Description,
            CreatedAt = userProject.Project.CreatedAt,
            UpdatedAt = userProject.Project.UpdatedAt,
            Role = userProject.Role.ToString()
        };
    }

    public async Task<List<ProjectUserDto>> GetProjectUsersAsync(Guid userId, Guid projectId)
    {
        // Verify user has access to this project
        var hasAccess = await _dbContext.UserProjects
            .AnyAsync(up => up.UserId == userId && up.ProjectId == projectId);

        if (!hasAccess)
        {
            return [];
        }

        var users = await _dbContext.UserProjects
            .Where(up => up.ProjectId == projectId)
            .Include(up => up.User)
            .Select(up => new ProjectUserDto
            {
                UserId = up.UserId,
                Email = up.User.Email,
                Name = up.User.Name,
                PictureUrl = up.User.PictureUrl,
                Role = up.Role.ToString(),
                JoinedAt = up.JoinedAt
            })
            .OrderBy(u => u.Role)
            .ThenBy(u => u.Email)
            .ToListAsync();

        return users;
    }

    public async Task<bool> DeleteProjectAsync(Guid userId, Guid projectId)
    {
        var userProject = await _dbContext.UserProjects
            .Where(up => up.UserId == userId && up.ProjectId == projectId)
            .Include(up => up.Project)
            .FirstOrDefaultAsync();

        if (userProject == null)
        {
            return false;
        }

        // Only Owner can delete the project
        if (userProject.Role != ProjectRole.Owner)
        {
            return false;
        }

        _dbContext.Projects.Remove(userProject.Project);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
