namespace ElementalAlchemist.Server;

/// <summary>Paths to the game content exported from Unity into shared/Content.</summary>
public sealed class ContentOptions
{
    public string ElementsPath { get; init; } = string.Empty;
    public string RecipesPath { get; init; } = string.Empty;
    public string TagsPath { get; init; } = string.Empty;
}
