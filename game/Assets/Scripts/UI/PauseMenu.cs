using ElementalAlchemist.Audio;
using ElementalAlchemist.GameInput;
using ElementalAlchemist.Progression;
using ElementalAlchemist.Save;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ElementalAlchemist.UI
{
    /// <summary>In-game pause menu: Resume, or Quit to the main menu (saving, or warning first in the tutorial).</summary>
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private string _mainMenuScene = "MainMenu";
        [SerializeField] private GameObject _backdropPanel;
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _quitButton;

        [SerializeField] private GameObject _tutorialQuitWindow;
        [SerializeField] private Button _tutorialQuitConfirmButton;
        [SerializeField] private Button _tutorialQuitCancelButton;

        private InputAction _pauseAction;
        private bool _isOpen;

        private void Awake()
        {
            _pauseAction = InputSystem.actions.FindAction(InputActions.Global.Pause);
            _backdropPanel.SetActive(false);
            _menuPanel.SetActive(false);
            SetTutorialQuitPromptActive(false);
        }

        private void OnEnable()
        {
            _pauseAction.performed += OnPause;
            _resumeButton.onClick.AddListener(Close);
            _quitButton.onClick.AddListener(OnQuit);
            _tutorialQuitConfirmButton.onClick.AddListener(OnTutorialQuitConfirmed);
            _tutorialQuitCancelButton.onClick.AddListener(OnTutorialQuitCancelled);
        }

        private void OnDisable()
        {
            _pauseAction.performed -= OnPause;
            _resumeButton.onClick.RemoveListener(Close);
            _quitButton.onClick.RemoveListener(OnQuit);
            _tutorialQuitConfirmButton.onClick.RemoveListener(OnTutorialQuitConfirmed);
            _tutorialQuitCancelButton.onClick.RemoveListener(OnTutorialQuitCancelled);
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

            // The tutorial does not save (see SaveManager), so warn before quitting that it must be replayed.
            var progression = ProgressionManager.Instance;
            if (progression && progression.CurrentStage == ProgressionStage.Tutorial)
            {
                SetTutorialQuitPromptActive(true);
                return;
            }

            QuitToMenu();
        }

        private void OnTutorialQuitConfirmed()
        {
            AudioManager.Click();
            QuitToMenu();
        }

        private void OnTutorialQuitCancelled()
        {
            AudioManager.Click();
            SetTutorialQuitPromptActive(false);
        }

        private void QuitToMenu()
        {
            Time.timeScale = 1f;

            // Save on the way out (the pause menu's only save point). No-op during the tutorial.
            if (SaveManager.Instance)
            {
                SaveManager.Instance.Save();
            }

            SceneManager.LoadScene(_mainMenuScene);
        }

        private void SetTutorialQuitPromptActive(bool active)
        {
            if (_tutorialQuitWindow)
            {
                _tutorialQuitWindow.SetActive(active);
            }
        }
    }
}
