using ElementalAlchemist.Shared;

namespace ElementalAlchemist.Server.Data;

public class Element
{
    public required string Id { get; init; }
    public required string DisplayName { get; init; }
    public required string Description { get; init; }
    public required ElementTier Tier { get; init; }
    public required string[] Tags { get; init; }
    public required DateTime CreatedAt { get; init; }
    
    public ElementDefinition ToDefinition() => new()
    {
        Id = Id,
        DisplayName = DisplayName,
        Description = Description,
        Tier = Tier,
        Tags = Tags,
    };
}