using ElementalAlchemist.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI
{
    /// <summary>
    /// Displays a single element stack in the inventory grid.
    /// </summary>
    public class ElementSlot : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _quantity;

        /// <summary>
        /// Populates the slot with data from an element stack.
        /// </summary>
        public void Setup(ElementStack stack)
        {
            _icon.sprite = stack.element.icon;
            _quantity.text = stack.quantity.ToString();
        }
    }
}