using Deckle.Domain.Data;
using Deckle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Deckle.API.Services;

public class DataSourceService
{
    private readonly AppDbContext _dbContext;
    private readonly GoogleSheetsService _googleSheetsService;

    public DataSourceService(AppDbContext dbContext, GoogleSheetsService googleSheetsService)
    {
        _dbContext = dbContext;
        _googleSheetsService = googleSheetsService;
    }

    public async Task<List<DataSourceDto>> GetProjectDataSourcesAsync(Guid userId, Guid projectId)
    {
        // Verify user has access to the project
        var hasAccess = await _dbContext.UserProjects
            .AnyAsync(up => up.UserId == userId && up.ProjectId == projectId);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("User does not have access to this project");
        }

        var dataSources = await _dbContext.DataSources
            .Where(ds => ds.ProjectId == projectId)
            .Select(ds => new DataSourceDto
            {
                Id = ds.Id,
                ProjectId = ds.ProjectId,
                Name = ds.Name,
                Type = ds.Type.ToString(),
                GoogleSheetsId = ds.GoogleSheetsId,
                GoogleSheetsUrl = ds.GoogleSheetsUrl,
                CreatedAt = ds.CreatedAt,
                UpdatedAt = ds.UpdatedAt
            })
            .ToListAsync();

        return dataSources;
    }

    public async Task<DataSourceDto?> GetDataSourceByIdAsync(Guid userId, Guid dataSourceId)
    {
        var dataSource = await _dbContext.DataSources
            .Include(ds => ds.Project)
                .ThenInclude(p => p.UserProjects)
            .FirstOrDefaultAsync(ds => ds.Id == dataSourceId);

        if (dataSource == null)
        {
            return null;
        }

        // Verify user has access to the project
        var hasAccess = dataSource.Project.UserProjects.Any(up => up.UserId == userId);
        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("User does not have access to this data source");
        }

        return new DataSourceDto
        {
            Id = dataSource.Id,
            ProjectId = dataSource.ProjectId,
            Name = dataSource.Name,
            Type = dataSource.Type.ToString(),
            GoogleSheetsId = dataSource.GoogleSheetsId,
            GoogleSheetsUrl = dataSource.GoogleSheetsUrl,
            CreatedAt = dataSource.CreatedAt,
            UpdatedAt = dataSource.UpdatedAt
        };
    }

    public async Task<DataSourceDto> CreateGoogleSheetsDataSourceAsync(Guid userId, Guid projectId, string name, string url)
    {
        // Verify user has access to the project
        var hasAccess = await _dbContext.UserProjects
            .AnyAsync(up => up.UserId == userId && up.ProjectId == projectId);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("User does not have access to this project");
        }

        // Extract spreadsheet ID from URL
        var spreadsheetId = GoogleSheetsService.ExtractSpreadsheetIdFromUrl(url);
        if (string.IsNullOrEmpty(spreadsheetId))
        {
            throw new ArgumentException("Invalid Google Sheets URL");
        }

        // Verify the user can access this spreadsheet
        try
        {
            var metadata = await _googleSheetsService.GetSpreadsheetMetadataAsync(userId, spreadsheetId);

            // Use spreadsheet title as name if not provided
            if (string.IsNullOrEmpty(name))
            {
                name = metadata.Title;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unable to access Google Sheet: {ex.Message}", ex);
        }

        var dataSource = new DataSource
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = name,
            Type = DataSourceType.GoogleSheets,
            ConnectionString = url,
            GoogleSheetsId = spreadsheetId,
            GoogleSheetsUrl = url,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.DataSources.Add(dataSource);
        await _dbContext.SaveChangesAsync();

        return new DataSourceDto
        {
            Id = dataSource.Id,
            ProjectId = dataSource.ProjectId,
            Name = dataSource.Name,
            Type = dataSource.Type.ToString(),
            GoogleSheetsId = dataSource.GoogleSheetsId,
            GoogleSheetsUrl = dataSource.GoogleSheetsUrl,
            CreatedAt = dataSource.CreatedAt,
            UpdatedAt = dataSource.UpdatedAt
        };
    }

    public async Task<bool> DeleteDataSourceAsync(Guid userId, Guid dataSourceId)
    {
        var dataSource = await _dbContext.DataSources
            .Include(ds => ds.Project)
                .ThenInclude(p => p.UserProjects)
            .FirstOrDefaultAsync(ds => ds.Id == dataSourceId);

        if (dataSource == null)
        {
            return false;
        }

        // Verify user has access to the project
        var hasAccess = dataSource.Project.UserProjects.Any(up => up.UserId == userId);
        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("User does not have access to this data source");
        }

        _dbContext.DataSources.Remove(dataSource);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}

public record DataSourceDto
{
    public required Guid Id { get; init; }
    public required Guid ProjectId { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public string? GoogleSheetsId { get; init; }
    public string? GoogleSheetsUrl { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}

public record CreateDataSourceRequest(Guid ProjectId, string Name, string Url);
