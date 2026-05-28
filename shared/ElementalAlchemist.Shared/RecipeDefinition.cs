namespace ElementalAlchemist.Shared;

public sealed record RecipeDefinition
{
    public required string ElementA { get; init; }
    public required string ElementB { get; init; }
    public required string Result { get; init; }
}
