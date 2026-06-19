using ElementalAlchemist.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ElementalAlchemist.UI
{
    /// <summary>Main menu controller: New Game, Continue (only when a save exists), and Quit.</summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string _newGameScene = "Village";
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _quitButton;

        private void OnEnable()
        {
            _newGameButton.onClick.AddListener(OnNewGame);
            _continueButton.onClick.AddListener(OnContinue);
            _quitButton.onClick.AddListener(OnQuit);
        }

        private void OnDisable()
        {
            _newGameButton.onClick.RemoveListener(OnNewGame);
            _continueButton.onClick.RemoveListener(OnContinue);
            _quitButton.onClick.RemoveListener(OnQuit);
        }

        private void Start()
        {
            // Continue only makes sense when there is a save to resume.
            _continueButton.interactable = SaveManager.HasSave();
        }

        private void OnNewGame()
        {
            if (SaveManager.Instance)
            {
                SaveManager.Instance.DeleteSave();
            }

            SceneManager.LoadScene(_newGameScene);
        }

        private void OnContinue()
        {
            if (SaveManager.Instance)
            {
                SaveManager.Instance.Load();
            }
        }

        private void OnQuit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
