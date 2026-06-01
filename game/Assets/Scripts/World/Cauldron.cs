using ElementalAlchemist.Dialogue;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.World
{
    public class Cauldron : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _triggerId = "cauldron-filled";
        [SerializeField] private GameObject _water;
        [SerializeField] private DialogueData _gateDialogue;

        private Collider _collider;

        public string Prompt => "Fill cauldron";

        public void Interact()
        {
            if (!StoryGate.TryProceed(_triggerId, _gateDialogue))
            {
                return;
            }

            _water.SetActive(true);
            _collider.enabled = false;
            ProgressionManager.Instance.OnWaterUnlocked();
            StoryTrigger.Fire(_triggerId);
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }
        
        private void Start()
        {
            var unlocked = ProgressionManager.Instance.HasWater;
            _water.SetActive(unlocked);
            _collider.enabled = !unlocked;
        }
    }
}