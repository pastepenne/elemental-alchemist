using System;
using ElementalAlchemist.GameInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ElementalAlchemist.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        public static event Action<string> DialogueStarted;
        public static event Action DialogueEnded;
        public static event Action<string> LineAdvanced;

        private DialogueData _currentDialogue;
        private int _lineIndex;
        private InputAction _advanceAction;

        public void StartDialogue(DialogueData dialogue)
        {
            if (_currentDialogue)
            {
                return;
            }
            
            _currentDialogue = dialogue;
            _lineIndex = 0;

            ActionMapController.SetActionMap(ActionMaps.Dialogue);
            _advanceAction.started += OnAdvance;

            DialogueStarted?.Invoke(_currentDialogue.Speaker);
            LineAdvanced?.Invoke(_currentDialogue.Lines[_lineIndex]);
        }

        private void Advance()
        {
            _lineIndex++;

            if (_lineIndex >= _currentDialogue.Lines.Length)
            {
                EndDialogue();
                return;
            }

            LineAdvanced?.Invoke(_currentDialogue.Lines[_lineIndex]);
        }

        private void EndDialogue()
        {
            _advanceAction.started -= OnAdvance;
            _currentDialogue = null;

            ActionMapController.SetActionMap(ActionMaps.Player);
            DialogueEnded?.Invoke();
        }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _advanceAction = InputSystem.actions.FindAction(InputActions.Dialogue.Advance);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnAdvance(InputAction.CallbackContext context)
        {
            Advance();
        }
    }
}