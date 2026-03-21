using System.Collections.Generic;
using ElementalAlchemist.GameInput;
using ElementalAlchemist.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ElementalAlchemist.UI
{
    /// <summary>
    /// Toggles the inventory panel and refreshes its contents.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject _backdropPanel;
        [SerializeField] private GameObject _windowPanel;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Transform _gridParent;

        private readonly List<GameObject> _slots = new();
        private InputAction _toggleAction;

        private void Awake()
        {
            _toggleAction = InputSystem.actions.FindAction(InputActions.Global.ToggleInventory);
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
        }

        private void OnEnable()
        {
            _toggleAction.performed += OnToggleInventory;
        }

        private void OnDisable()
        {
            _toggleAction.performed -= OnToggleInventory;
        }

        private void OnToggleInventory(InputAction.CallbackContext context)
        {
            var opening = !_windowPanel.activeSelf;
            _backdropPanel.SetActive(opening);
            _windowPanel.SetActive(opening);

            if (opening)
            {
                ActionMapController.SetActionMap(ActionMaps.UI);
                PlayerManager.Instance.PlayerInventory.OnInventoryChanged += RefreshGrid;
                RefreshGrid();
            }
            else
            {
                ActionMapController.SetActionMap(ActionMaps.Player);
                PlayerManager.Instance.PlayerInventory.OnInventoryChanged -= RefreshGrid;
            }
        }

        private void RefreshGrid()
        {
            foreach (var slot in _slots)
            {
                Destroy(slot);
            }
            
            _slots.Clear();

            foreach (var stack in PlayerManager.Instance.PlayerInventory.GetStacks())
            {
                var slotObject = Instantiate(_slotPrefab, _gridParent);
                slotObject.GetComponent<ElementSlot>().Setup(stack);
                _slots.Add(slotObject);
            }
        }
    }
}
