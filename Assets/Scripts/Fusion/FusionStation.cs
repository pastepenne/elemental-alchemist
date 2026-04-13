using System;
using UnityEngine;

namespace ElementalAlchemist.Fusion
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
