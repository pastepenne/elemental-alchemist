using ElementalAlchemist.Dialogue;
using TMPro;
using UnityEngine;

namespace ElementalAlchemist.UI.Dialogue
{
    public class DialogueDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _speakerText;
        [SerializeField] private TMP_Text _lineText;

        private void Awake()
        {
            _panel.SetActive(false);
        }

        private void OnEnable()
        {
            DialogueManager.DialogueStarted += OnDialogueStarted;
            DialogueManager.DialogueEnded += OnDialogueEnded;
            DialogueManager.LineAdvanced += OnLineAdvanced;
        }

        private void OnDisable()
        {
            DialogueManager.DialogueStarted -= OnDialogueStarted;
            DialogueManager.DialogueEnded -= OnDialogueEnded;
            DialogueManager.LineAdvanced -= OnLineAdvanced;
        }

        private void OnDialogueStarted(string speaker)
        {
            _speakerText.text = speaker;
            _panel.SetActive(true);
        }

        private void OnDialogueEnded()
        {
            _panel.SetActive(false);
            _speakerText.text = "";
            _lineText.text = "";
        }

        private void OnLineAdvanced(string line)
        {
            _lineText.text = line;
        }
    }
}
