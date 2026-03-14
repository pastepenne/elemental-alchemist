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
    
    [CreateAssetMenu(fileName = "New Element", menuName = "Elemental Alchemist/Element")]
    public class Element : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public ElementTier tier;
    }
}
