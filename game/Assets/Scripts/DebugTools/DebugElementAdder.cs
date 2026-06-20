using System.Collections.Generic;
using System.Linq;
using ElementalAlchemist.Element;
using ElementalAlchemist.Player;
using ElementalAlchemist.Shared;
using UnityEngine;

namespace ElementalAlchemist.DebugTools
{
    public class DebugElementAdder : MonoBehaviour
    {
        [SerializeField] private ElementRegistry _registry;
        [SerializeField] private ElementData _requiredElement;

        [ContextMenu("Debug/Add Required Element")]
        private void AddRequiredElement()
        {
            if (_requiredElement)
            {
                AddElements(new[] { _requiredElement });
            }
        }

        [ContextMenu("Debug/Add Natural Elements")]
        private void AddNaturalElements() => AddElements(_registry.Elements.Where(e => e.Tier == ElementTier.Natural));

        [ContextMenu("Debug/Add Refined Elements")]
        private void AddRefinedElements() => AddElements(_registry.Elements.Where(e => e.Tier == ElementTier.Refined));

        [ContextMenu("Debug/Add Advanced Elements")]
        private void AddAdvancedElements() => AddElements(_registry.Elements.Where(e => e.Tier == ElementTier.Advanced));

        [ContextMenu("Debug/Add All Elements")]
        private void AddAllElements() => AddElements(_registry.Elements);

        private static void AddElements(IEnumerable<ElementData> elements)
        {
            if (!PlayerManager.Instance || PlayerManager.Instance.Inventory is not { } inventory)
            {
                Debug.LogWarning("[DebugElementAdder] Player inventory is missing.");
                return;
            }

            foreach (var element in elements)
            {
                inventory.AddElement(element);
                Debug.Log($"[DebugElementAdder] Added {element.name}");
            }
        }
    }
}
