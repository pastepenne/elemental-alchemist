using ElementalAlchemist.Data;
using ElementalAlchemist.Player;
using UnityEngine;

namespace ElementalAlchemist.Interaction
{
    public class ElementSource : MonoBehaviour, IInteractable
    {
        [SerializeField] private int _yield = 1;
        [SerializeField] private Element _element;

        public string Prompt => "Collect";
        
        public void Interact()
        {
            PlayerManager.Instance.Inventory.AddElement(_element, _yield);
            Destroy(gameObject);
        }
    }
}