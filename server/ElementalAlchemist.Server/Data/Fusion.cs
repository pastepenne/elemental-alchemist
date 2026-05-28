namespace ElementalAlchemist.Server.Data;

public class Fusion
{
    public long Id { get; init; }
    public required string ElementA { get; init; }
    public required string ElementB { get; init; }
    public required string? ResultId { get; init; }
    public required DateTime CreatedAt { get; init; }

    // Navigation property to the resulting element
    public Element? Result { get; init; }
}
