namespace ElementalAlchemist.Server.Generation;

/// <summary>Raw element data parsed from the model's JSON output.</summary>
public sealed record GenerationResult
{
    public bool Possible { get; init; }
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string[]? Tags { get; init; }

    public static GenerationResult Impossible() => new() { Possible = false };
}
