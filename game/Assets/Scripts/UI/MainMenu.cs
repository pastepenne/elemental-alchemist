using ElementalAlchemist.Audio;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using ElementalAlchemist.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ElementalAlchemist.UI
{
    /// <summary>Main menu controller: New Game (confirms before overwriting an existing save), Continue (only when a
    /// save exists), and Quit.</summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string _newGameScene = "Village";
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _quitButton;

        [SerializeField] private GameObject _overwriteBackdrop;
        [SerializeField] private GameObject _overwriteWindow;
        [SerializeField] private Button _overwriteYesButton;
        [SerializeField] private Button _overwriteNoButton;

        private void Awake()
        {
            // Quitting to the menu leaves the gameplay singletons alive (DontDestroyOnLoad). Clear them here —
            // the gameplay scene and its camera are already gone, so this can't collide with a scene transition —
            // so New Game / Continue start from a clean slate. SaveManager is intentionally kept.
            ClearManager(PlayerManager.Instance);
            ClearManager(ProgressionManager.Instance);
            ClearManager(StoryDirector.Instance);

            SetOverwritePromptActive(false);
        }

        private void SetOverwritePromptActive(bool active)
        {
            if (_overwriteBackdrop)
            {
                _overwriteBackdrop.SetActive(active);
            }

            if (_overwriteWindow)
            {
                _overwriteWindow.SetActive(active);
            }
        }

        private static void ClearManager(Component manager)
        {
            if (manager)
            {
                manager.gameObject.SetActive(false);
                Destroy(manager.gameObject);
            }
        }

        private void OnEnable()
        {
            _newGameButton.onClick.AddListener(OnNewGame);
            _continueButton.onClick.AddListener(OnContinue);
            _quitButton.onClick.AddListener(OnQuit);
            _overwriteYesButton.onClick.AddListener(OnOverwriteConfirmed);
            _overwriteNoButton.onClick.AddListener(OnOverwriteCancelled);
        }

        private void OnDisable()
        {
            _newGameButton.onClick.RemoveListener(OnNewGame);
            _continueButton.onClick.RemoveListener(OnContinue);
            _quitButton.onClick.RemoveListener(OnQuit);
            _overwriteYesButton.onClick.RemoveListener(OnOverwriteConfirmed);
            _overwriteNoButton.onClick.RemoveListener(OnOverwriteCancelled);
        }

        private void Start()
        {
            // Continue only makes sense when there is a save to resume.
            _continueButton.interactable = SaveManager.HasSave();
        }

        private void OnNewGame()
        {
            AudioManager.Click();

            // An existing save would be overwritten by a fresh run, so make the player confirm first.
            if (SaveManager.HasSave())
            {
                SetOverwritePromptActive(true);
                return;
            }

            StartNewGame();
        }

        private void OnOverwriteConfirmed()
        {
            AudioManager.Click();
            StartNewGame();
        }

        private void OnOverwriteCancelled()
        {
            AudioManager.Click();
            SetOverwritePromptActive(false);
        }

        private void StartNewGame()
        {
            if (SaveManager.Instance)
            {
                SaveManager.Instance.DeleteSave();
            }

            SceneManager.LoadScene(_newGameScene);
        }

        private void OnContinue()
        {
            AudioManager.Click();

            if (SaveManager.Instance)
            {
                SaveManager.Instance.Load();
            }
        }

        private void OnQuit()
        {
            AudioManager.Click();
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
