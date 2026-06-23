using ElementalAlchemist.Dialogue;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ElementalAlchemist.World
{
    public class SceneTransition : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private string _arriveAtSpawnId;
        [SerializeField] private string _prompt = "Enter";
        [SerializeField] private ProgressionStage _requiredStage = ProgressionStage.Tutorial;
        [SerializeField] private DialogueData _gateDialogue;

        public string Prompt => _prompt;

        public void Interact()
        {
            if (string.IsNullOrEmpty(_sceneName))
            {
                Debug.LogWarning($"No scene name set on {name}.", this);
                return;
            }

            if (ProgressionManager.Instance.CurrentStage < _requiredStage)
            {
                if (_gateDialogue)
                {
                    DialogueManager.Instance.StartDialogue(_gateDialogue);
                }
                return;
            }

            PlayerSpawner.PendingSpawnId = _arriveAtSpawnId;
            SceneManager.LoadScene(_sceneName);
        }
    }
}
