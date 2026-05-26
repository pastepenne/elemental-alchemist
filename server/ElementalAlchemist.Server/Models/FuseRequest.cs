namespace ElementalAlchemist.Server.Models;

/// <summary>Incoming fusion request from the game: two element names to combine.</summary>
public record FuseRequest(string ElementA, string ElementB);
