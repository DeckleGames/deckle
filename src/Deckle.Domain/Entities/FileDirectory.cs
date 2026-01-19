namespace Deckle.Domain.Entities;

public class FileDirectory
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ParentDirectoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!;
    public FileDirectory? ParentDirectory { get; set; }
    public ICollection<FileDirectory> ChildDirectories { get; set; } = [];
    public ICollection<File> Files { get; set; } = [];
}
