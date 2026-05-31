namespace ElementalAlchemist.Server.Generation;

public sealed class GenerationOptions
{
    public string Model { get; init; } = "claude-haiku-4-5-20251001";
    public int MaxTokens { get; init; } = 400;
}
