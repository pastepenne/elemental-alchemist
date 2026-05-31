using System;
using System.Collections.Generic;
using ElementalAlchemist.Shared;
using UnityEngine;

namespace ElementalAlchemist.Element
{
    [CreateAssetMenu(fileName = "New Element Registry", menuName = "Elemental Alchemist/Element Registry")]
    public class ElementRegistry : ScriptableObject
    {
        [SerializeField] private ElementData[] _elements;
        [SerializeField] private TagSpriteLibrary _tagSprites;

        private Dictionary<string, ElementData> _lookup;

        private void OnEnable()
        {
            _lookup = new Dictionary<string, ElementData>(StringComparer.Ordinal);

            if (_elements == null)
            {
                return;
            }

            foreach (var element in _elements)
            {
                if (element && !string.IsNullOrEmpty(element.Id))
                {
                    _lookup[element.Id] = element;
                }
            }
        }
        
        public ElementData Resolve(ElementDefinition definition)
        {
            if (definition == null || string.IsNullOrEmpty(definition.Id))
            {
                return null;
            }

            _lookup ??= new Dictionary<string, ElementData>(StringComparer.Ordinal);

            if (_lookup.TryGetValue(definition.Id, out var existing) && existing)
            {
                return existing;
            }

            var icon = BuildIcon(definition.Tags);
            var tier = definition.Tier;

            var element = ElementData.CreateRuntime(
                definition.Id,
                definition.DisplayName,
                definition.Description,
                tier,
                icon,
                definition.Tags);

            _lookup[definition.Id] = element;
            return element;
        }
        
        private Sprite BuildIcon(string[] tags)
        {
            if (!_tagSprites)
            {
                return null;
            }

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    if (!string.IsNullOrEmpty(tag))
                    {
                        return _tagSprites.GetSprite(tag);
                    }
                }
            }

            return _tagSprites.Fallback;
        }
    }
}
