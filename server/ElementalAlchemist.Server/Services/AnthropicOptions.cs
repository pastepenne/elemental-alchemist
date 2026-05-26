namespace ElementalAlchemist.Server.Services;

/// <summary>Anthropic API settings bound from the "Anthropic" configuration section.</summary>
public sealed class AnthropicOptions
{
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "claude-haiku-4-5-20251001";
    public string Version { get; set; } = "2023-06-01";
}
