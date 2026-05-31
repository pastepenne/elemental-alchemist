namespace ElementalAlchemist.Shared;

public sealed record RecipeDefinition
{
    public string ElementA { get; set; } = string.Empty;
    public string ElementB { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
}
