using ElementalAlchemist.Element;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Pouch
{
    public class PouchDetails : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        
        public void Display(ElementStack stack)
        {
            _icon.sprite = stack.Element.Icon;
            _name.text = stack.Element.DisplayName;
            _description.text = stack.Element.Description;
        }
    }
}