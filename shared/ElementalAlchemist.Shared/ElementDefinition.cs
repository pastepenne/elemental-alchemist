namespace ElementalAlchemist.Shared
{
    public record ElementDefinition
    {
        public required string Id { get; init; }
        public required string DisplayName { get; init; }
        public required string Description { get; init; }
        public required ElementTier Tier { get; init; }
        public required string[] Tags { get; init; }
    }
}
