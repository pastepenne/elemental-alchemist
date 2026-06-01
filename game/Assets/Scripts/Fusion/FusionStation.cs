using System;
using ElementalAlchemist.Dialogue;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.Fusion
{
    public class FusionStation : MonoBehaviour, IInteractable
    {
        public static event Action Interacted;

        [SerializeField] private DialogueData _gateDialogue;

        public string Prompt => "Fuse";

        public void Interact()
        {
            if (!ProgressionManager.Instance.HasAllCores)
            {
                if (_gateDialogue)
                {
                    DialogueManager.Instance.StartDialogue(_gateDialogue);
                }
                return;
            }

            Interacted?.Invoke();
        }
    }
}
