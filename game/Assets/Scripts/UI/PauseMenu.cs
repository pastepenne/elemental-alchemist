using ElementalAlchemist.Audio;
using ElementalAlchemist.GameInput;
using ElementalAlchemist.Save;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ElementalAlchemist.UI
{
    /// <summary>In-game pause menu: Resume, or Quit to the main menu (which saves). Freezes time while shown.</summary>
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private string _mainMenuScene = "MainMenu";
        [SerializeField] private GameObject _backdropPanel;
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _quitButton;

        private InputAction _pauseAction;
        private bool _isOpen;

        private void Awake()
        {
            _pauseAction = InputSystem.actions.FindAction(InputActions.Global.Pause);
            _backdropPanel.SetActive(false);
            _menuPanel.SetActive(false);
        }

        private void OnEnable()
        {
            _pauseAction.performed += OnPause;
            _resumeButton.onClick.AddListener(Close);
            _quitButton.onClick.AddListener(OnQuit);
        }

        private void OnDisable()
        {
            _pauseAction.performed -= OnPause;
            _resumeButton.onClick.RemoveListener(Close);
            _quitButton.onClick.RemoveListener(OnQuit);
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            if (_isOpen)
            {
                Close();
                return;
            }

            // Only pausable from normal gameplay (not during cutscenes, dialogue, or another menu).
            if (ActionMapController.Current != ActionMaps.Player)
            {
                return;
            }

            Open();
        }

        private void Open()
        {
            _isOpen = true;
            AudioManager.PauseOpen();
            _backdropPanel.SetActive(true);
            _menuPanel.SetActive(true);
            ActionMapController.SetActionMap(ActionMaps.UI);
            Time.timeScale = 0f;
        }

        private void Close()
        {
            _isOpen = false;
            AudioManager.Click();
            _backdropPanel.SetActive(false);
            _menuPanel.SetActive(false);
            Time.timeScale = 1f;
            ActionMapController.SetActionMap(ActionMaps.Player);
        }

        private void OnQuit()
        {
            AudioManager.Click();
            Time.timeScale = 1f;

            // Save on the way out (the pause menu's only save point).
            if (SaveManager.Instance)
            {
                SaveManager.Instance.Save();
            }

            SceneManager.LoadScene(_mainMenuScene);
        }
    }
}
