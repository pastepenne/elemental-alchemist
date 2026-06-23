using System.Collections.Generic;
using System.Linq;
using ElementalAlchemist.Element;
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
        [SerializeField] private GameObject _emptyState;

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
                .OrderBy(e => e.Tier)
                .ThenBy(e => e.DisplayName)
                .ToList();

            UpdateEmptyState(discoveredElements.Count == 0);

            foreach (var element in discoveredElements)
            {
                var entryObject = Instantiate(_entryPrefab, _contentGroup.transform);
                var entryComponent = entryObject.GetComponent<TomeEntry>();
                entryComponent.Setup(element, _contentGroup);
                entryComponent.EntrySelected += OnEntrySelected;
                _entries.Add(entryObject);
            }
        }

        private void UpdateEmptyState(bool isEmpty)
        {
            if (_emptyState)
            {
                _emptyState.SetActive(isEmpty);
            }

            if (_contentGroup.gameObject)
            {
                _contentGroup.gameObject.SetActive(!isEmpty);
            }

            if (_details.gameObject)
            {
                _details.gameObject.SetActive(!isEmpty);
            }
        }

        private void OnEntrySelected(ElementData element)
        {
            _details.Display(element);
        }
    }
}
