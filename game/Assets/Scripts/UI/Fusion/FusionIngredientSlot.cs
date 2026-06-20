using ElementalAlchemist.Audio;
using ElementalAlchemist.Element;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Fusion
{
    [RequireComponent(typeof(Button))]
    public class FusionIngredientSlot : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _background;
        [SerializeField] private TierColorPalette _tierColors;
        [SerializeField] private Color _emptyColor = new(0.6f, 0.5f, 0.4f, 1f);

        private Button _button;
        private ElementData _element;
        
        public ElementData Current { get; private set; }

        public void Setup(ElementData element)
        {
            Current = element;
            _icon.sprite = element.Icon;
            
            _icon.gameObject.SetActive(true);
            _background.color = _tierColors.GetColor(element.Tier);
        }
        
        public void Clear()
        {
            _icon.gameObject.SetActive(false);
            _background.color = _emptyColor;
            Current = null;
        }
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClicked);
            
            Clear();
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
            if (!Current)
            {
                return;
            }

            AudioManager.Click();
            Clear();
        }
    }
}
