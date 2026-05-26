namespace ElementalAlchemist.Server.Models;

/// <summary>Fusion outcome returned to the game.</summary>
public record FuseResponse(
    bool Possible,
    string? Name,
    string? Description,
    string? Tier,
    string[] Tags);
