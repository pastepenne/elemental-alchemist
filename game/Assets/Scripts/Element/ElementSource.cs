using ElementalAlchemist.Audio;
using ElementalAlchemist.Player;
using UnityEngine;

namespace ElementalAlchemist.Element
{
    public class ElementSource : MonoBehaviour, IInteractable
    {
        [SerializeField] private int _yield = 1;
        [SerializeField] private ElementData _element;

        public string Prompt => "Collect";

        public void Interact()
        {
            PlayerManager.Instance.Inventory.AddElement(_element, _yield);
            AudioManager.Pickup();
            Destroy(gameObject);
        }
    }
}