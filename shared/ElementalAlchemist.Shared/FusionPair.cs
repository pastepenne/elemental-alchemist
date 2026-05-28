namespace ElementalAlchemist.Shared;

public sealed record FusionPair
{
    public string ElementA { get; }
    public string ElementB { get; }
    
    public FusionPair(string elementA, string elementB)
    {
        if (string.CompareOrdinal(elementA, elementB) <= 0)
        {
            ElementA = elementA;
            ElementB = elementB;
        }
        else
        {
            ElementA = elementB;
            ElementB = elementA;
        }
    }
}
