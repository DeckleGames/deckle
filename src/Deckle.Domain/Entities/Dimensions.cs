using Deckle.Utils.Helpers;

namespace Deckle.Domain.Entities;

public class Dimensions
{

    public required decimal WidthMm { get; init; }
    public required decimal HeightMm { get; init; }
    public required decimal BleedMm { get; init; }
    public int Dpi { get; set; } = 300;
    public int WidthPx => SizeHelper.MmToPx(WidthMm, Dpi);
    public int HeightPx => SizeHelper.MmToPx(HeightMm, Dpi);
    public int BleedPx => SizeHelper.MmToPx(BleedMm, Dpi);
}