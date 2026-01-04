namespace Deckle.Domain.Entities;

public class PlayerMat : Component, IEditableComponent, IDataSourceComponent
{
    // If PresetSize is set, use it with Orientation
    // Otherwise, use CustomWidthMm and CustomHeightMm
    public PlayerMatSize? PresetSize { get; set; }

    public PlayerMatOrientation Orientation { get; set; } = PlayerMatOrientation.Portrait;

    public decimal? CustomWidthMm { get; set; }

    public decimal? CustomHeightMm { get; set; }

    public string? FrontDesign { get; set; }

    public string? BackDesign { get; set; }

    public ComponentShape Shape { get; set; } = new RectangleShape(0);

    public DataSource? DataSource { get; set; }

    public Dimensions GetDimensions()
    {
        if (PresetSize.HasValue)
        {
            return PresetSize.Value.GetDimensions(Orientation);
        }
        else
        {
            return new()
            {
                WidthMm = CustomWidthMm!.Value,
                HeightMm = CustomHeightMm!.Value,
                BleedMm = 3
            };
        }
    }
}
