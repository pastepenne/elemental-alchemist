using System.Collections.Generic;
using ElementalAlchemist.Data;
using ElementalAlchemist.Player;
using UnityEngine;

namespace ElementalAlchemist.UI
{
    /// <summary>
    /// Displays the player's element inventory as a grid of slots.
    /// </summary>
    public class InventoryTab : MonoBehaviour
    {
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Transform _gridParent;

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
                Destroy(slot);
            }

            _slots.Clear();

            foreach (var stack in PlayerManager.Instance.Inventory.GetStacks())
            {
                var slotObject = Instantiate(_slotPrefab, _gridParent);
                slotObject.GetComponent<ElementSlot>().Setup(stack);
                _slots.Add(slotObject);
            }
        }
    }
}
