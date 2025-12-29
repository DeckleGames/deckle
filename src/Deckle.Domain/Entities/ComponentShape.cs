using System.Text.Json.Serialization;

namespace Deckle.Domain.Entities;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RectangleShape), "rectangle")]
public abstract class ComponentShape
{
}

public class RectangleShape : ComponentShape
{
    public int BorderRadiusMm { get; set; }

    public RectangleShape()
    {
    }

    public RectangleShape(int borderRadiusMm)
    {
        BorderRadiusMm = borderRadiusMm;
    }
}
