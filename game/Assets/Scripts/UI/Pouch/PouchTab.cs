using System.Collections.Generic;
using ElementalAlchemist.Element;
using ElementalAlchemist.Player;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Pouch
{
    /// <summary>
    /// Displays the player's element inventory as a grid of slots.
    /// </summary>
    public class PouchTab : MonoBehaviour
    {
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private ToggleGroup _contentGroup;
        [SerializeField] private PouchDetails _details;
        [SerializeField] private GameObject _emptyState;

        private readonly List<GameObject> _slots = new();

        private void OnEnable()
        {
            PlayerManager.Instance.Inventory.ElementAdded += OnInventoryChanged;
            PlayerManager.Instance.Inventory.ElementRemoved += OnInventoryChanged;
            RefreshGrid();
        }

        private void OnDisable()
        {
            PlayerManager.Instance.Inventory.ElementAdded -= OnInventoryChanged;
            PlayerManager.Instance.Inventory.ElementRemoved -= OnInventoryChanged;
        }

        private void OnInventoryChanged(ElementData _)
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            foreach (var slot in _slots)
            {
                var slotComponent = slot.GetComponent<PouchSlot>();
                if (slotComponent)
                {
                    slotComponent.StackSelected -= OnStackSelected;
                }
                
                Destroy(slot);
            }

            _slots.Clear();

            var stacks = PlayerManager.Instance.Inventory.GetStacks();

            UpdateEmptyState(stacks.Count == 0);

            foreach (var stack in stacks)
            {
                var slotObject = Instantiate(_slotPrefab, _contentGroup.transform);
                var slotComponent = slotObject.GetComponent<PouchSlot>();
                slotComponent.Setup(stack, _contentGroup);
                slotComponent.StackSelected += OnStackSelected;
                _slots.Add(slotObject);
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
        
        private void OnStackSelected(ElementStack element)
        {
            _details.Display(element);
        }
    }
}
