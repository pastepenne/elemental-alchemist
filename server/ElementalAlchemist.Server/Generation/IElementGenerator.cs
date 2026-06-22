using ElementalAlchemist.Shared;

namespace ElementalAlchemist.Server.Generation;

/// <summary>Generates a new element definition from two input element names.</summary>
public interface IElementGenerator
{
    Task<ElementDefinition?> GenerateAsync(string a, string b, CancellationToken ct);
}
