using System.Collections.Generic;
using ElementalAlchemist.Element;
using ElementalAlchemist.Player;
using UnityEngine;

namespace ElementalAlchemist.DebugTools
{
    /// <summary>Editor cheat: right-click the component to drop the listed elements into the player's pouch.</summary>
    public class DebugElementAdder : MonoBehaviour
    {
        [SerializeField] private List<ElementData> _elements = new();

        [ContextMenu("Debug/Add Elements")]
        private void AddElements()
        {
            var inventory = PlayerManager.Instance ? PlayerManager.Instance.Inventory : null;
            if (inventory == null)
            {
                Debug.LogWarning("[DebugElementAdder] No PlayerManager yet; enter Play mode first.");
                return;
            }

            foreach (var element in _elements)
            {
                if (element)
                {
                    // AddElement raises ElementAdded, which the Discovery service listens to, so this also
                    // marks the element discovered and satisfies guardian riddle checks.
                    inventory.AddElement(element);
                    Debug.Log($"[DebugElementAdder] Added {element.name}");
                }
            }
        }
    }
}
