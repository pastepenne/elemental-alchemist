using ElementalAlchemist.GameInput;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ElementalAlchemist.UI
{
    /// <summary>
    /// Manages the player menu panel with tabbed navigation (Inventory, Discovery).
    /// </summary>
    public class PlayerMenu : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _backdropPanel;
        [SerializeField] private GameObject _windowPanel;

        [Header("Tabs")]
        [SerializeField] private Button _inventoryTabButton;
        [SerializeField] private Button _discoveryTabButton;
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private GameObject _discoveryPanel;

        private InputAction _toggleAction;
        private bool _isOpen;

        private void Awake()
        {
            _toggleAction = InputSystem.actions.FindAction(InputActions.Global.ToggleInventory);
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
        }

        private void OnEnable()
        {
            _toggleAction.performed += OnToggle;
        }

        private void OnDisable()
        {
            _toggleAction.performed -= OnToggle;
        }

        private void OnToggle(InputAction.CallbackContext context)
        {
            if (_isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        private void Open()
        {
            _isOpen = true;
            _backdropPanel.SetActive(true);
            _windowPanel.SetActive(true);
            ActionMapController.SetActionMap(ActionMaps.UI);
            ShowInventoryTab();
        }

        private void Close()
        {
            _isOpen = false;
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
            ActionMapController.SetActionMap(ActionMaps.Player);
        }

        private void ShowInventoryTab()
        {
            _inventoryPanel.SetActive(true);
            _discoveryPanel.SetActive(false);
        }

        private void ShowDiscoveryTab()
        {
            _inventoryPanel.SetActive(false);
            _discoveryPanel.SetActive(true);
        }
    }
}
