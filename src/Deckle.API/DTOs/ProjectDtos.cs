namespace Deckle.API.DTOs;

public record ProjectDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required string Role { get; init; }
}

public record CreateProjectRequest(string Name, string? Description);
