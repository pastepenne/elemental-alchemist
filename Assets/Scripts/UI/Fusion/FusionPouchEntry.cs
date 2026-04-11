using System;
using ElementalAlchemist.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Fusion
{
    public class FusionPouchEntry : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;

        private Button _button;
        private Element _element;

        public event Action<Element> ElementSelected;

        public void Setup(Element element)
        {
            _element = element;
            _icon.sprite = element.icon;
            _name.text = element.displayName;
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
