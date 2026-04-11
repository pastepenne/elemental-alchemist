using System;
using ElementalAlchemist.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Pouch
{
    /// <summary>
    /// Displays a single element stack in the inventory grid.
    /// </summary>
    public class PouchSlot : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _quantity;
        [SerializeField] private TierColorPalette _tierColors;
        
        private Toggle _toggle;
        private ElementStack _stack;
        
        public event Action<ElementStack> StackSelected;
        
        /// <summary>
        /// Populates the slot with data from an element stack.
        /// </summary>
        public void Setup(ElementStack stack, ToggleGroup toggleGroup)
        {
            _stack = stack;
            _icon.sprite = stack.element.icon;
            _quantity.text = stack.quantity.ToString();
            _background.color = _tierColors.GetColor(stack.element.tier);
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
            if (isOn && _stack != null && _stack.element)
            {
                StackSelected?.Invoke(_stack);
            }
        }
    }
}