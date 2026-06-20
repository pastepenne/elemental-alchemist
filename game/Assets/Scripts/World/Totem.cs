using ElementalAlchemist.Dialogue;
using UnityEngine;

namespace ElementalAlchemist.World
{
    /// <summary>Ruins riddle totem: speaks a hint dialogue about a harder fusion when examined.</summary>
    public class Totem : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueData _riddle;

        public string Prompt => "Examine";

        public void Interact()
        {
            if (_riddle)
            {
                DialogueManager.Instance.StartDialogue(_riddle);
            }
        }
    }
}
