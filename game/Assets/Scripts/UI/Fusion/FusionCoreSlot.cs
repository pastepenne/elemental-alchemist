using System;
using ElementalAlchemist.Element;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Fusion
{
    [RequireComponent(typeof(Button))]
    public class FusionCoreSlot : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _background;
        [SerializeField] private TierColorPalette _tierColors;
        
        private Button _button;
        private ElementData _element;

        public event Action<ElementData> ElementSelected;
        
        public void Setup(ElementData element)
        {
            _element = element;
            _icon.sprite = element.Icon;
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
                ElementSelected?.Invoke(_element);
            }
        }
    }
}