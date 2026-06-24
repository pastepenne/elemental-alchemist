using ElementalAlchemist.Element;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Pouch
{
    public class PouchDetails : MonoBehaviour
    {
        [SerializeField] private TagSpriteLibrary _tagSprites;
        [SerializeField] private TierColorPalette _tierColors;
        [SerializeField] private Image[] _icons;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _tier;
        [SerializeField] private Image _tierBackground;

        public void Display(ElementStack stack)
        {
            ElementIcons.Apply(_icons, stack.Element, _tagSprites);
            _name.text = stack.Element.DisplayName;
            _description.text = stack.Element.Description;
            _tier.text = stack.Element.Tier.ToString();
            _tierBackground.color = _tierColors.GetColor(stack.Element.Tier);
        }
    }
}