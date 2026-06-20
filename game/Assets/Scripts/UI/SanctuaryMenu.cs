using ElementalAlchemist.Dialogue;
using ElementalAlchemist.GameInput;
using ElementalAlchemist.Progression;
using ElementalAlchemist.World;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ElementalAlchemist.UI
{
    /// <summary>The Life ritual confirm menu opened at the Sanctuary altar. "Fuse Life" can never succeed - the gate
    /// dialogue stays the player's hand and the menu returns; "Walk away" hands off to the epilogue sequence, whose
    /// opening dialogue is the Grand Master's furious farewell and whose step then walks him out of the realm.</summary>
    public class SanctuaryMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _backdropPanel;
        [SerializeField] private GameObject _windowPanel;
        [SerializeField] private Button _fuseLifeButton;
        [SerializeField] private Button _walkAwayButton;
        [SerializeField] private DialogueData _gateDialogue;
        [SerializeField] private StorySequence _epilogueSequence;

        private InputAction _cancelAction;
        private bool _isOpen;

        private void Awake()
        {
            _cancelAction = InputSystem.actions.FindAction(InputActions.UI.Cancel);
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
        }

        private void OnEnable()
        {
            SanctuaryAltar.Interacted += Open;
            _fuseLifeButton.onClick.AddListener(OnFuseLife);
            _walkAwayButton.onClick.AddListener(OnWalkAway);
        }

        private void OnDisable()
        {
            SanctuaryAltar.Interacted -= Open;
            _fuseLifeButton.onClick.RemoveListener(OnFuseLife);
            _walkAwayButton.onClick.RemoveListener(OnWalkAway);
            DialogueManager.DialogueEnded -= ReopenAfterGate;
            DialogueManager.DialogueEnded -= OnFarewellSpoken;

            if (_isOpen)
            {
                _cancelAction.performed -= OnCancel;
            }
        }

        private void Open()
        {
            _isOpen = true;
            _backdropPanel.SetActive(true);
            _windowPanel.SetActive(true);
            ActionMapController.SetActionMap(ActionMaps.UI);
            _cancelAction.performed += OnCancel;
        }

        private void Close()
        {
            if (!_isOpen)
            {
                return;
            }

            _isOpen = false;
            _backdropPanel.SetActive(false);
            _windowPanel.SetActive(false);
            _cancelAction.performed -= OnCancel;
            ActionMapController.SetActionMap(ActionMaps.Player);
        }

        private void OnFuseLife()
        {
            // The "yes" path is a trap: the gate dialogue stays the player's hand, then the menu reopens.
            Close();

            if (_gateDialogue)
            {
                DialogueManager.DialogueEnded += ReopenAfterGate;
                DialogueManager.Instance.StartDialogue(_gateDialogue);
            }
            else
            {
                Open();
            }
        }

        private void ReopenAfterGate()
        {
            DialogueManager.DialogueEnded -= ReopenAfterGate;
            Open();
        }

        private void OnWalkAway()
        {
            Close();

            // Hand off to the epilogue sequence. Its opening dialogue is the Grand Master's farewell - he is already
            // at the altar, so he speaks it on the spot. Once he is done, advance the sequence (fire its step's
            // trigger) so the step walks him out and its ActivateFreeplay outcome opens freeplay. We wait for the
            // dialogue to end first: firing immediately would flip the action map to Cutscene mid-sentence.
            if (StoryDirector.Instance && _epilogueSequence)
            {
                StoryDirector.Instance.BeginSequence(_epilogueSequence);
                DialogueManager.DialogueEnded += OnFarewellSpoken;
            }
        }

        private void OnFarewellSpoken()
        {
            DialogueManager.DialogueEnded -= OnFarewellSpoken;

            var steps = _epilogueSequence.Steps;
            if (steps != null && steps.Length > 0)
            {
                StoryTrigger.Fire(steps[0].TriggerId);
            }
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            // Backing out is not a decision; the altar will offer the choice again on the next interaction.
            Close();
        }
    }
}
