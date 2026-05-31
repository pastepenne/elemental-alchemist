namespace ElementalAlchemist.Server;

public static class ElementTags
{
    public static readonly HashSet<string> All = new(StringComparer.Ordinal)
    {
        "fire", "water", "air", "earth", "wood", "stone",
        "metal", "ice", "lightning", "shadow", "light", "poison",
    };
}
