using ElementalAlchemist.Dialogue;
using UnityEngine;

namespace ElementalAlchemist.Characters
{
    public class Npc : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueData _dialogueData;
        public string Prompt { get; } = "Talk";
        public void Interact()
        {
            DialogueManager.Instance.StartDialogue(_dialogueData);
        }
    }
}