namespace Deckle.API.DTOs;

public record SpreadsheetMetadata
{
    public required string SpreadsheetId { get; init; }
    public required string Title { get; init; }
    public required List<SheetMetadata> Sheets { get; init; }
}

public record SheetMetadata
{
    public required int SheetId { get; init; }
    public required string Title { get; init; }
    public required int RowCount { get; init; }
    public required int ColumnCount { get; init; }
}
