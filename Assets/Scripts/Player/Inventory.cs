using System;
using System.Collections.Generic;
using System.Linq;
using ElementalAlchemist.Data;

namespace ElementalAlchemist.Player
{
    /// <summary>
    /// Manages the player's collection of elements.
    /// </summary>
    public class Inventory
    {
        private readonly List<ElementStack> _stacks = new();

        /// <summary>
        /// Fired whenever an element is added to the inventory.
        /// </summary>
        public event Action<Element> ElementAdded;

        /// <summary>
        /// Fired whenever an element is removed from the inventory.
        /// </summary>
        public event Action<Element> ElementRemoved;

        /// <summary>
        /// Returns a read-only view of all element stacks.
        /// </summary>
        public IReadOnlyList<ElementStack> GetStacks() => _stacks;

        /// <summary>
        /// Adds an element to the inventory. Stacks with existing elements of the same type.
        /// </summary>
        public void AddElement(Element element, int quantity = 1)
        {
            if (!element || quantity <= 0)
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

            ElementAdded?.Invoke(element);
        }

        /// <summary>
        /// Removes a quantity of an element from the inventory. Returns true if successful.
        /// </summary>
        public bool RemoveElement(Element element, int quantity = 1)
        {
            if (!element || quantity <= 0)
            {
                return false;
            }

            var stack = _stacks.FirstOrDefault(s => s.element == element);
            if (stack == null || stack.quantity < quantity)
            {
                return false;
            }

            stack.quantity -= quantity;
            if (stack.quantity <= 0)
            {
                _stacks.Remove(stack);
            }

            ElementRemoved?.Invoke(element);
            return true;
        }

        /// <summary>
        /// Returns whether the inventory contains at least the given quantity of an element.
        /// </summary>
        public bool HasElement(Element element, int quantity = 1)
        {
            var stack = _stacks.FirstOrDefault(s => s.element == element);
            return stack != null && stack.quantity >= quantity;
        }
    }
}