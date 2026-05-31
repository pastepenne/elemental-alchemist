using UnityEngine;
using UnityEngine.SceneManagement;

namespace ElementalAlchemist
{
    public class SceneTransition : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private string _prompt = "Enter";

        public string Prompt => _prompt;

        public void Interact()
        {
            if (string.IsNullOrEmpty(_sceneName))
            {
                Debug.LogWarning($"No scene name set on {name}.", this);
                return;
            }

            SceneManager.LoadScene(_sceneName);
        }
    }
}
