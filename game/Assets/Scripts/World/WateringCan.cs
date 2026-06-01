using ElementalAlchemist.Dialogue;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.World
{
    public class WateringCan: MonoBehaviour, IInteractable
    {
        [SerializeField] private string _triggerId = "water-fetched";
        [SerializeField] private DialogueData _gateDialogue;

        public string Prompt => "Take watering can";

        public void Interact()
        {
            if (!StoryGate.TryProceed(_triggerId, _gateDialogue))
            {
                return;
            }

            StoryTrigger.Fire(_triggerId);
            Destroy(gameObject);
        }

        private void Start()
        {
            if (ProgressionManager.Instance.HasWater)
            {
                Destroy(gameObject);
            }
        }
    }
}