using System;
using System.Collections.Generic;
using System.Linq;
using ElementalAlchemist.Data;

namespace ElementalAlchemist.Player
{
    /// <summary>
    /// Manages the player's collection of elements.
    /// </summary>
    public class PlayerInventory
    {
        private readonly List<ElementStack> _stacks = new();

        /// <summary>
        /// Fired whenever the inventory contents change.
        /// </summary>
        public event Action OnInventoryChanged;

        /// <summary>
        /// Returns a read-only view of all element stacks.
        /// </summary>
        public IReadOnlyList<ElementStack> GetStacks() => _stacks;

        /// <summary>
        /// Adds an element to the inventory. Stacks with existing elements of the same type.
        /// </summary>
        public void AddElement(Element element, int quantity = 1)
        {
            if (element == null || quantity <= 0)
            {
                return;
            }

            var existingStack = _stacks.FirstOrDefault(stack => stack.element == element);
            if (existingStack != null)
            {
                existingStack.quantity += quantity;
            }
            else
            {
                _stacks.Add(new ElementStack(element, quantity));
            }

            OnInventoryChanged?.Invoke();
        }
    }
}