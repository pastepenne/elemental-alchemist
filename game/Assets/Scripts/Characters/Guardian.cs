using ElementalAlchemist.Dialogue;
using ElementalAlchemist.Element;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.Characters
{
    /// <summary>Realm guardian NPC. Drives its own beat of the active story sequence: poses a riddle on the first
    /// meeting, re-hints until the player has discovered the answer element, then congratulates and advances.</summary>
    public class Guardian : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _guardianName;
        [SerializeField] private DialogueData _riddleDialogue;
        [SerializeField] private DialogueData _completedDialogue;
        [SerializeField] private DialogueData _idleDialogue;

        [SerializeField] private ElementData _requiredElement;
        [SerializeField] private string _metTriggerId;
        [SerializeField] private string _claimTriggerId;

        private string _triggerOnDialogueEnd;

        public string Prompt => "Talk";

        public void Interact()
        {
            var director = StoryDirector.Instance;

            if (director && director.IsCurrentTrigger(_metTriggerId))
            {
                // First meeting: pose the riddle, then mark the guardian as found.
                PlayThenFire(_riddleDialogue, _metTriggerId);
            }
            else if (director && director.IsCurrentTrigger(_claimTriggerId))
            {
                if (HasRequiredElement())
                {
                    // Riddle solved: congratulate, then advance the story (grants the fragment).
                    PlayThenFire(_completedDialogue, _claimTriggerId);
                }
                else
                {
                    // Riddle not solved yet: re-hint without advancing.
                    DialogueManager.Instance.StartDialogue(_riddleDialogue);
                }
            }
            else
            {
                DialogueManager.Instance.StartDialogue(_idleDialogue);
            }
        }

        private bool HasRequiredElement()
        {
            return _requiredElement && PlayerManager.Instance &&
                   PlayerManager.Instance.Discovery.IsElementDiscovered(_requiredElement);
        }

        private void PlayThenFire(DialogueData dialogue, string triggerId)
        {
            if (_triggerOnDialogueEnd != null)
            {
                // A trigger is already pending from a prior interaction; ignore re-entry.
                return;
            }

            _triggerOnDialogueEnd = triggerId;
            DialogueManager.DialogueEnded += OnDialogueEnded;
            DialogueManager.Instance.StartDialogue(dialogue);
        }

        private void OnDialogueEnded()
        {
            DialogueManager.DialogueEnded -= OnDialogueEnded;
            var triggerId = _triggerOnDialogueEnd;
            _triggerOnDialogueEnd = null;
            StoryTrigger.Fire(triggerId);
        }

        private void OnDisable()
        {
            if (_triggerOnDialogueEnd != null)
            {
                DialogueManager.DialogueEnded -= OnDialogueEnded;
                _triggerOnDialogueEnd = null;
            }
        }
    }
}
