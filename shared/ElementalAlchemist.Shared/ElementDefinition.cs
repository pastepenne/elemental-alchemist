namespace ElementalAlchemist.Shared;

public record ElementDefinition
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ElementTier Tier { get; set; }
    public string[] Tags { get; set; } = System.Array.Empty<string>();
}
