using UnityEngine;

namespace ElementalAlchemist.Data
{
    public enum ElementTier
    {
        Basic,
        Compound,
        Advanced,
        Exotic
    }
    
    [CreateAssetMenu(fileName = "New Tier Color Palette", menuName = "Elemental Alchemist/Tier Color Palette")]
    public class TierColorPalette : ScriptableObject
    {
        [SerializeField] private Color _basic = new(0.804f, 0.498f, 0.196f);
        [SerializeField] private Color _compound = new(0.753f, 0.753f, 0.753f);
        [SerializeField] private Color _advanced = new(1f, 0.843f, 0f);
        [SerializeField] private Color _exotic = new(0.18f, 0.8f, 0.44f);

        public Color GetColor(ElementTier tier) => tier switch
        {
            ElementTier.Basic => _basic,
            ElementTier.Compound => _compound,
            ElementTier.Advanced => _advanced,
            ElementTier.Exotic => _exotic,
            _ => Color.white
        };
    }
}