using ElementalAlchemist.Dialogue;
using ElementalAlchemist.Element;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.World
{
    public class Branch : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _triggerId = "branch-collected";
        [SerializeField] private ElementData _wood;
        [SerializeField] private DialogueData _gateDialogue;

        public string Prompt => "Collect branch";

        public void Interact()
        {
            if (!StoryGate.TryProceed(_triggerId, _gateDialogue))
            {
                return;
            }
            
            PlayerManager.Instance.Inventory.AddElement(_wood, 1);

            StoryTrigger.Fire(_triggerId);
            Destroy(gameObject);
        }

        private void Start()
        {
            if (ProgressionManager.Instance.HasFire)
            {
                Destroy(gameObject);
            }
        }
    }
}