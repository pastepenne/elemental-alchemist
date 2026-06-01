using ElementalAlchemist.Dialogue;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.World
{
    public class Shovel : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _triggerId = "shovel-taken";
        [SerializeField] private DialogueData _gateDialogue;

        public string Prompt => "Take shovel";

        public void Interact()
        {
            if (!StoryGate.TryProceed(_triggerId, _gateDialogue))
            {
                return;
            }

            ProgressionManager.Instance.OnEarthUnlocked();
            StoryTrigger.Fire(_triggerId);
            Destroy(gameObject);
        }

        private void Start()
        {
            if (ProgressionManager.Instance.HasEarth)
            {
                Destroy(gameObject);
            }
        }
    }
}