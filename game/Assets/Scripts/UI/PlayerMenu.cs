using ElementalAlchemist.Audio;
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
        [SerializeField] private Button _closeButton;

        private InputAction _toggleAction;
        private InputAction _cancelAction;
        private bool _isOpen;

        private void Awake()
        {
            _toggleAction = InputSystem.actions.FindAction(InputActions.Global.TogglePlayerMenu);
            _cancelAction = InputSystem.actions.FindAction(InputActions.UI.Cancel);
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
        }

        private void OnEnable()
        {
            _toggleAction.performed += OnToggle;
            _pouchTabButton.onClick.AddListener(OnPouchTabClicked);
            _tomeTabButton.onClick.AddListener(OnTomeTabClicked);
            _closeButton.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            _toggleAction.performed -= OnToggle;
            _pouchTabButton.onClick.RemoveListener(OnPouchTabClicked);
            _tomeTabButton.onClick.RemoveListener(OnTomeTabClicked);
            _closeButton.onClick.RemoveListener(Close);
        }

        private void Open()
        {
            _isOpen = true;
            AudioManager.PlayerMenuOpen();
            _backdropPanel.SetActive(true);
            _windowPanel.SetActive(true);
            ActionMapController.SetActionMap(ActionMaps.UI);
            _cancelAction.performed += OnCancel;
            ShowPouchTab();
        }

        private void Close()
        {
            _isOpen = false;
            AudioManager.Click();
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
            ActionMapController.SetActionMap(ActionMaps.Player);
            _cancelAction.performed -= OnCancel;
        }

        // Tab presses click; ShowPouchTab/ShowTomeTab stay silent so Open()'s programmatic call does not double up.
        private void OnPouchTabClicked()
        {
            AudioManager.Click();
            ShowPouchTab();
        }

        private void OnTomeTabClicked()
        {
            AudioManager.Click();
            ShowTomeTab();
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
        
        private void OnToggle(InputAction.CallbackContext context)
        {
            if (_isOpen)
            {
                Close();
                return;
            }

            if (ActionMapController.Current != ActionMaps.Player)
            {
                return;
            }

            Open();
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            Close();
        }
    }
}
