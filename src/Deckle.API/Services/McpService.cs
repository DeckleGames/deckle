using Deckle.API.DTOs;
using Deckle.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Deckle.API.Services;

/// <summary>
/// Service for handling Model Context Protocol (MCP) operations
/// Allows AI agents to interact with Deckle data
/// </summary>
public class McpService
{
    private readonly AppDbContext _dbContext;
    private readonly ComponentService _componentService;
    private readonly DataSourceService _dataSourceService;

    public McpService(
        AppDbContext dbContext,
        ComponentService componentService,
        DataSourceService dataSourceService)
    {
        _dbContext = dbContext;
        _componentService = componentService;
        _dataSourceService = dataSourceService;
    }

    /// <summary>
    /// Get server information for initialization
    /// </summary>
    public McpInitializeResult GetInitializeResult()
    {
        return new McpInitializeResult
        {
            ProtocolVersion = "2024-11-05",
            ServerInfo = new McpServerInfo
            {
                Name = "Deckle MCP Server",
                Version = "1.0.0"
            },
            Capabilities = new McpServerCapabilities
            {
                Tools = new { }
            }
        };
    }

    /// <summary>
    /// Get list of available tools
    /// </summary>
    public McpToolsListResult GetToolsList()
    {
        return new McpToolsListResult
        {
            Tools =
            [
                new McpTool
                {
                    Name = "list_projects",
                    Description = "List all projects accessible to the authenticated user. Returns project ID, name, description, role, and timestamps.",
                    InputSchema = new
                    {
                        type = "object",
                        properties = new { },
                        required = Array.Empty<string>()
                    }
                },
                new McpTool
                {
                    Name = "get_project_details",
                    Description = "Get detailed information about a specific project, including all components (cards and dice) and linked data sources.",
                    InputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            project_id = new
                            {
                                type = "string",
                                description = "The unique identifier (GUID) of the project"
                            }
                        },
                        required = new[] { "project_id" }
                    }
                }
            ]
        };
    }

    /// <summary>
    /// Execute a tool call
    /// </summary>
    public async Task<McpToolCallResult> ExecuteToolAsync(Guid userId, string toolName, Dictionary<string, object>? arguments)
    {
        try
        {
            return toolName switch
            {
                "list_projects" => await ListProjectsAsync(userId),
                "get_project_details" => await GetProjectDetailsAsync(userId, arguments),
                _ => new McpToolCallResult
                {
                    Content =
                    [
                        new McpContent
                        {
                            Type = "text",
                            Text = $"Unknown tool: {toolName}"
                        }
                    ],
                    IsError = true
                }
            };
        }
        catch (Exception ex)
        {
            return new McpToolCallResult
            {
                Content =
                [
                    new McpContent
                    {
                        Type = "text",
                        Text = $"Error executing tool: {ex.Message}"
                    }
                ],
                IsError = true
            };
        }
    }

    /// <summary>
    /// List all projects for a user
    /// </summary>
    private async Task<McpToolCallResult> ListProjectsAsync(Guid userId)
    {
        var projects = await _dbContext.UserProjects
            .Where(up => up.UserId == userId)
            .Include(up => up.Project)
            .Select(up => new McpProjectSummary
            {
                Id = up.Project.Id.ToString(),
                Name = up.Project.Name,
                Description = up.Project.Description,
                Role = up.Role.ToString(),
                CreatedAt = up.Project.CreatedAt,
                UpdatedAt = up.Project.UpdatedAt
            })
            .ToListAsync();

        var resultText = projects.Count == 0
            ? "No projects found."
            : JsonSerializer.Serialize(projects, new JsonSerializerOptions
            {
                WriteIndented = true
            });

        return new McpToolCallResult
        {
            Content =
            [
                new McpContent
                {
                    Type = "text",
                    Text = resultText
                }
            ],
            IsError = false
        };
    }

    /// <summary>
    /// Get detailed information about a project
    /// </summary>
    private async Task<McpToolCallResult> GetProjectDetailsAsync(Guid userId, Dictionary<string, object>? arguments)
    {
        // Extract project_id from arguments
        if (arguments == null || !arguments.TryGetValue("project_id", out var projectIdObj))
        {
            return new McpToolCallResult
            {
                Content =
                [
                    new McpContent
                    {
                        Type = "text",
                        Text = "Missing required argument: project_id"
                    }
                ],
                IsError = true
            };
        }

        var projectIdString = projectIdObj?.ToString();
        if (string.IsNullOrEmpty(projectIdString) || !Guid.TryParse(projectIdString, out var projectId))
        {
            return new McpToolCallResult
            {
                Content =
                [
                    new McpContent
                    {
                        Type = "text",
                        Text = "Invalid project_id format. Must be a valid GUID."
                    }
                ],
                IsError = true
            };
        }

        // Get project basic info
        var project = await _dbContext.UserProjects
            .Where(up => up.UserId == userId && up.ProjectId == projectId)
            .Include(up => up.Project)
            .Select(up => new
            {
                up.Project.Id,
                up.Project.Name,
                up.Project.Description,
                Role = up.Role.ToString(),
                up.Project.CreatedAt,
                up.Project.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (project == null)
        {
            return new McpToolCallResult
            {
                Content =
                [
                    new McpContent
                    {
                        Type = "text",
                        Text = $"Project not found or you don't have access to it. Project ID: {projectId}"
                    }
                ],
                IsError = true
            };
        }

        // Get components
        var components = await _componentService.GetProjectComponentsAsync(userId, projectId);
        var componentSummaries = components.Select(c => new McpComponentSummary
        {
            Id = c.Id.ToString(),
            Name = c.Name,
            Type = c.Type,
            Size = c is CardDto card ? card.Size : null,
            DiceType = c is DiceDto dice ? dice.Type : null,
            Number = c is DiceDto d ? d.Number : null
        }).ToList();

        // Get data sources
        var dataSources = await _dataSourceService.GetProjectDataSourcesAsync(userId, projectId);
        var dataSourceSummaries = dataSources.Select(ds => new McpDataSourceSummary
        {
            Id = ds.Id.ToString(),
            Name = ds.Name,
            Type = ds.Type,
            GoogleSheetsUrl = ds.GoogleSheetsUrl
        }).ToList();

        var projectDetails = new McpProjectDetails
        {
            Id = project.Id.ToString(),
            Name = project.Name,
            Description = project.Description,
            Role = project.Role,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            Components = componentSummaries,
            DataSources = dataSourceSummaries
        };

        var resultText = JsonSerializer.Serialize(projectDetails, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return new McpToolCallResult
        {
            Content =
            [
                new McpContent
                {
                    Type = "text",
                    Text = resultText
                }
            ],
            IsError = false
        };
    }
}
