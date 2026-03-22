using System;
using UnityEngine;

namespace ElementalAlchemist.Interaction
{
    public class FusionStation : MonoBehaviour, IInteractable
    {
        public static event Action Interacted;

        public string Prompt => "Fuse";

        public void Interact()
        {
            Interacted?.Invoke();
        }
    }
}
