using ElementalAlchemist.Shared;
using UnityEngine;

namespace ElementalAlchemist.Element
{
    [CreateAssetMenu(fileName = "New Element", menuName = "Elemental Alchemist/Element")]
    public class ElementData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] [TextArea] private string _description;
        [SerializeField] private ElementTier _tier;
        [SerializeField] private string[] _tags;

        public string Id => _id;
        public string DisplayName => _displayName;
        public string Description  => _description;
        public ElementTier Tier => _tier;
        public string[] Tags => _tags;
        public bool IsCore => Tier == ElementTier.Core;
        public bool IsDynamic { get; private set; }

        public ElementDefinition ToDefinition() => new()
        {
            Id = _id,
            DisplayName = _displayName,
            Description = _description,
            Tier = _tier,
            Tags = _tags
        };

        /// <summary>
        /// Creates an in-memory ElementData for a server-provided element.
        /// </summary>
        public static ElementData CreateRuntime(
            string id,
            string displayName,
            string description,
            ElementTier tier,
            string[] tags)
        {
            var element = CreateInstance<ElementData>();
            element.IsDynamic = true;
            element._id = id;
            element._displayName = displayName;
            element._description = description;
            element._tier = tier;
            element._tags = tags ?? System.Array.Empty<string>();
            element.name = id;
            return element;
        }
    }
}
