using System;
using ElementalAlchemist.Audio;
using ElementalAlchemist.Element;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Tome
{
    /// <summary>
    /// Displays a single discovered element with the recipes used to obtain it.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class TomeEntry : MonoBehaviour
    {
        [SerializeField] private Image[] _icons;
        [SerializeField] private TagSpriteLibrary _tagSprites;
        [SerializeField] private TMP_Text _name;

        private Toggle _toggle;
        private ElementData _element;

        public event Action<ElementData> EntrySelected;

        public void Setup(ElementData element, ToggleGroup toggleGroup)
        {
            _element = element;
            ElementIcons.Apply(_icons, element, _tagSprites);
            _name.text = element.DisplayName;
            _toggle.group = toggleGroup;
        }

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        
        private void OnDestroy()
        {
            if (_toggle)
            {
                _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (isOn && _element)
            {
                AudioManager.Click();
                EntrySelected?.Invoke(_element);
            }
        }
    }
}
