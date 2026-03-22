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
        [SerializeField] private GameObject _backdropPanel;
        [SerializeField] private GameObject _windowPanel;
        [SerializeField] private Button _pouchTabButton;
        [SerializeField] private Button _tomeTabButton;
        [SerializeField] private GameObject _pouchView;
        [SerializeField] private GameObject _tomeView;

        private InputAction _toggleAction;
        private bool _isOpen;

        private void Awake()
        {
            _toggleAction = InputSystem.actions.FindAction(InputActions.Global.TogglePlayerMenu);
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
        }

        private void OnEnable()
        {
            _toggleAction.performed += OnToggle;
            _pouchTabButton.onClick.AddListener(ShowPouchTab);
            _tomeTabButton.onClick.AddListener(ShowTomeTab);
        }

        private void OnDisable()
        {
            _toggleAction.performed -= OnToggle;
            _pouchTabButton.onClick.RemoveListener(ShowPouchTab);
            _tomeTabButton.onClick.RemoveListener(ShowTomeTab);
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
            ShowPouchTab();
        }

        private void Close()
        {
            _isOpen = false;
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
            ActionMapController.SetActionMap(ActionMaps.Player);
        }

        private void ShowPouchTab()
        {
            // Toggle Views
            _pouchView.SetActive(true);
            _tomeView.SetActive(false);
            
            // Toggle Button States
            _pouchTabButton.interactable = false;
            _tomeTabButton.interactable = true;
        }

        private void ShowTomeTab()
        {
            // Toggle Views
            _pouchView.SetActive(false);
            _tomeView.SetActive(true);
            
            // Toggle Button States
            _pouchTabButton.interactable = true;
            _tomeTabButton.interactable = false;
        }
    }
}
