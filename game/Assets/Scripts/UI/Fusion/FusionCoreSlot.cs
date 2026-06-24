using System;
using ElementalAlchemist.Audio;
using ElementalAlchemist.Element;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Fusion
{
    [RequireComponent(typeof(Button))]
    public class FusionCoreSlot : MonoBehaviour
    {
        [SerializeField] private Image[] _icons;
        [SerializeField] private TagSpriteLibrary _tagSprites;
        [SerializeField] private Image _background;
        [SerializeField] private TierColorPalette _tierColors;
        
        private Button _button;
        private ElementData _element;

        public event Action<ElementData> ElementSelected;
        
        public void Setup(ElementData element)
        {
            _element = element;
            ElementIcons.Apply(_icons, element, _tagSprites);
            _background.color = _tierColors.GetColor(element.Tier);
        }
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClicked);
        }
        
        private void OnDestroy()
        {
            if (_button)
            {
                _button.onClick.RemoveListener(OnClicked);
            }
        }

        private void OnClicked()
        {
            if (_element)
            {
                AudioManager.Click();
                ElementSelected?.Invoke(_element);
            }
        }
    }
}