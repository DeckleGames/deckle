namespace Deckle.API.DTOs;

public record FileDirectoryDto(
    Guid Id,
    Guid ProjectId,
    Guid? ParentDirectoryId,
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record FileDirectoryWithContentsDto(
    Guid Id,
    Guid ProjectId,
    Guid? ParentDirectoryId,
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<FileDirectoryDto> ChildDirectories,
    List<FileDto> Files
);

public record CreateFileDirectoryRequest(
    string Name,
    Guid? ParentDirectoryId
);

public record RenameFileDirectoryRequest(
    string Name
);

public record MoveFileDirectoryRequest(
    Guid? ParentDirectoryId
);
