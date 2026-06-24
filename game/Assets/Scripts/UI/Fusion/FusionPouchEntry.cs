using System;
using ElementalAlchemist.Audio;
using ElementalAlchemist.Element;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Fusion
{
    [RequireComponent(typeof(Button))]
    public class FusionPouchEntry : MonoBehaviour
    {
        [SerializeField] private Image[] _icons;
        [SerializeField] private TagSpriteLibrary _tagSprites;
        [SerializeField] private TMP_Text _name;

        private Button _button;
        private ElementData _element;

        public event Action<ElementData> ElementSelected;

        public void Setup(ElementData element)
        {
            _element = element;
            ElementIcons.Apply(_icons, element, _tagSprites);
            _name.text = element.DisplayName;
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
