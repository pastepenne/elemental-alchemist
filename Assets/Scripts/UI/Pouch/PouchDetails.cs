using ElementalAlchemist.Data;
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
            _icon.sprite = stack.element.icon;
            _name.text = stack.element.displayName;
            _description.text = stack.element.description;
        }
    }
}