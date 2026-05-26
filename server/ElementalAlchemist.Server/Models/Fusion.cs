namespace ElementalAlchemist.Server.Models;

/// <summary>A persisted fusion of two elements. Maps to the <c>Fusions</c> table.</summary>
/// <remarks>
/// <see cref="ElementA"/> and <see cref="ElementB"/> are stored canonically (trimmed, lowercased,
/// alphabetically sorted) so lookups are order- and case-independent. A null <see cref="ResultName"/>
/// is the "no outcome" sentinel: the pair was generated but the AI deemed it impossible.
/// </remarks>
public class Fusion
{
    public int Id { get; set; }
    public string ElementA { get; set; } = "";
    public string ElementB { get; set; } = "";
    public string? ResultName { get; set; }
    public string? ResultDescription { get; set; }
    public string? ResultTier { get; set; }
    public string? ResultTags { get; set; }
    public string CreatedAt { get; set; } = "";
}
