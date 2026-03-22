using System.Collections.Generic;
using ElementalAlchemist.Data;
using ElementalAlchemist.Player;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI
{
    /// <summary>
    /// Displays the player's element inventory as a grid of slots.
    /// </summary>
    public class PouchTab : MonoBehaviour
    {
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private ToggleGroup _contentGroup;
        [SerializeField] private PouchDetails _details;

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

        private void OnInventoryChanged(Element _)
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

            foreach (var stack in PlayerManager.Instance.Inventory.GetStacks())
            {
                var slotObject = Instantiate(_slotPrefab, _contentGroup.transform);
                var slotComponent = slotObject.GetComponent<PouchSlot>();
                slotComponent.Setup(stack, _contentGroup);
                slotComponent.StackSelected += OnStackSelected;
                _slots.Add(slotObject);
            }
        }
        
        private void OnStackSelected(ElementStack element)
        {
            _details.Display(element);
        }
    }
}
