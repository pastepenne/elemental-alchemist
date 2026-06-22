using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElementalAlchemist.Element
{
    [CreateAssetMenu(fileName = "New Tag Sprite Library", menuName = "Elemental Alchemist/Tag Sprite Library")]
    public class TagSpriteLibrary : ScriptableObject
    {
        [Serializable]
        private struct Entry
        {
            public string tag;
            public Sprite sprite;
        }

        [SerializeField] private Entry[] _entries;
        [SerializeField] private Sprite _fallback;

        private Dictionary<string, Sprite> _lookup;

        public Sprite Fallback => _fallback;

        // The tags this library provides icons for; the single source of truth for valid element tags.
        public IEnumerable<string> Tags
        {
            get
            {
                if (_entries == null)
                {
                    yield break;
                }

                foreach (var entry in _entries)
                {
                    if (!string.IsNullOrEmpty(entry.tag))
                    {
                        yield return entry.tag;
                    }
                }
            }
        }

        private void OnEnable()
        {
            _lookup = new Dictionary<string, Sprite>(StringComparer.Ordinal);

            if (_entries == null)
            {
                return;
            }

            foreach (var entry in _entries)
            {
                if (!string.IsNullOrEmpty(entry.tag) && entry.sprite)
                {
                    _lookup[entry.tag] = entry.sprite;
                }
            }
        }

        public Sprite GetSprite(string tag)
        {
            if (_lookup == null)
            {
                OnEnable();
            }

            if (!string.IsNullOrEmpty(tag) && _lookup.TryGetValue(tag, out var sprite))
            {
                return sprite;
            }

            return _fallback;
        }
    }
}
