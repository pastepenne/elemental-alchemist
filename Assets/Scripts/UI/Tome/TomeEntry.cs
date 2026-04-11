using System;
using ElementalAlchemist.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Tome
{
    /// <summary>
    /// Displays a single discovered element with the recipes used to obtain it.
    /// </summary>
    public class TomeEntry : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;

        private Toggle _toggle;
        private Element _element;

        public event Action<Element> EntrySelected;

        public void Setup(Element element, ToggleGroup toggleGroup)
        {
            _element = element;
            _icon.sprite = element.icon;
            _name.text = element.displayName;
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
                EntrySelected?.Invoke(_element);
            }
        }
    }
}
