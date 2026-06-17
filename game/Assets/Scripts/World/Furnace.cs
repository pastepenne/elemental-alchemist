using ElementalAlchemist.Dialogue;
using ElementalAlchemist.Element;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.World
{
    [RequireComponent(typeof(Collider))]
    public class Furnace : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _triggerId = "furnace-started";
        [SerializeField] private GameObject _fire;
        [SerializeField] private ElementData _wood;
        [SerializeField] private DialogueData _gateDialogue;

        private Collider _collider;
        
        public string Prompt => "Throw wood";

        public void Interact()
        {
            if (!StoryGate.TryProceed(_triggerId, _gateDialogue))
            {
                return;
            }

            _fire.SetActive(true);
            _collider.enabled = false;
            PlayerManager.Instance.Inventory.RemoveElement(_wood, 3);
            ProgressionManager.Instance.OnFireUnlocked();
            StoryTrigger.Fire(_triggerId);
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }
        
        private void Start()
        {
            var unlocked = ProgressionManager.Instance.HasFire;
            _fire.SetActive(unlocked);
            _collider.enabled = !unlocked;
        }
    }
}