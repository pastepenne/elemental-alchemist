using ElementalAlchemist.Dialogue;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.World
{
    public class Windmill : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _triggerId = "windmill-started";
        [SerializeField] private DialogueData _gateDialogue;
        
        private Animator _animator;
        private Collider _collider;

        public string Prompt => "Start windmill";

        public void Interact()
        {
            if (!StoryGate.TryProceed(_triggerId, _gateDialogue))
            {
                return;
            }
            
            _animator.enabled = true;
            _collider.enabled = false;
            ProgressionManager.Instance.OnAirUnlocked();
            StoryTrigger.Fire(_triggerId);
        }
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
        }
        
        private void Start()
        {
            var unlocked = ProgressionManager.Instance.HasAir;
            _animator.enabled = unlocked;
            _collider.enabled = !unlocked;
        }
    }
}
