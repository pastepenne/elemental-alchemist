using System.Collections.Generic;
using System.Linq;
using ElementalAlchemist.Data;
using ElementalAlchemist.Player;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Tome
{
    /// <summary>
    /// Displays all discovered elements ordered by tier, with fusion recipes that produce them.
    /// </summary>
    public class TomeTab : MonoBehaviour
    {
        [SerializeField] private GameObject _entryPrefab;
        [SerializeField] private ToggleGroup _contentGroup;
        [SerializeField] private TomeDetails _details;

        private readonly List<GameObject> _entries = new();

        private void OnEnable()
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            foreach (var entry in _entries)
            {
                var entryComponent = entry.GetComponent<TomeEntry>();
                if (entryComponent)
                {
                    entryComponent.EntrySelected -= OnEntrySelected;
                }
                
                Destroy(entry);
            }

            _entries.Clear();

            var discoveredElements = PlayerManager.Instance.Discovery
                .GetDiscoveredElements()
                .OrderBy(e => e.tier)
                .ThenBy(e => e.displayName)
                .ToList();

            foreach (var element in discoveredElements)
            {
                var entryObject = Instantiate(_entryPrefab, _contentGroup.transform);
                var entryComponent = entryObject.GetComponent<TomeEntry>();
                entryComponent.Setup(element, _contentGroup);
                entryComponent.EntrySelected += OnEntrySelected;
                _entries.Add(entryObject);
            }
        }

        private void OnEntrySelected(Element element)
        {
            _details.Display(element);
        }
    }
}
