using UnityEngine;

namespace ElementalAlchemist.Element
{
    [CreateAssetMenu(fileName = "New Element", menuName = "Elemental Alchemist/Element")]
    public class ElementData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] [TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private ElementTier _tier;

        public string Id => _id;
        public string DisplayName => _displayName;
        public string Description  => _description;
        public Sprite Icon => _icon;
        public ElementTier Tier => _tier;
        public bool IsCore => Tier == ElementTier.Core;
    }
}
