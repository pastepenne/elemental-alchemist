using UnityEngine;

namespace ElementalAlchemist.Data
{
    public enum ElementTier
    {
        Core,
        Natural,
        Refined,
        Advanced,
        Exotic
    }
    
    [CreateAssetMenu(fileName = "New Tier Color Palette", menuName = "Elemental Alchemist/Tier Color Palette")]
    public class TierColorPalette : ScriptableObject
    {
        [SerializeField] private Color _core = new(0.9f, 0.9f, 0.85f);
        [SerializeField] private Color _natural = new(0.804f, 0.498f, 0.196f);
        [SerializeField] private Color _refined = new(0.7f, 0.75f, 0.85f);
        [SerializeField] private Color _advanced = new(1f, 0.843f, 0f);
        [SerializeField] private Color _exotic = new(0.18f, 0.8f, 0.44f);

        public Color GetColor(ElementTier tier) => tier switch
        {
            ElementTier.Core => _core,
            ElementTier.Natural => _natural,
            ElementTier.Refined => _refined,
            ElementTier.Advanced => _advanced,
            ElementTier.Exotic => _exotic,
            _ => Color.white
        };
    }
}