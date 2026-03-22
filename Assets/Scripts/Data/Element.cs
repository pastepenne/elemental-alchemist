using UnityEngine;

namespace ElementalAlchemist.Data
{
    [CreateAssetMenu(fileName = "New Element", menuName = "Elemental Alchemist/Element")]
    public class Element : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public ElementTier tier;

        public bool IsCore => tier == ElementTier.Core;
    }
}
