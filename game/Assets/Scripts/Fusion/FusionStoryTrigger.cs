using ElementalAlchemist.Element;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using ElementalAlchemist.UI.Fusion;
using UnityEngine;

namespace ElementalAlchemist.Fusion
{
    /// <summary>Fires a story trigger after the player fuses a target element and closes the fusion menu.</summary>
    public class FusionStoryTrigger : MonoBehaviour
    {
        [SerializeField] private ElementData _targetElement;
        [SerializeField] private string _triggerId;

        private bool _pending;

        private void OnEnable()
        {
            if (PlayerManager.Instance)
            {
                PlayerManager.Instance.Inventory.ElementAdded += OnElementAdded;
            }
            FusionMenu.Closed += OnMenuClosed;
        }

        private void OnDisable()
        {
            if (PlayerManager.Instance)
            {
                PlayerManager.Instance.Inventory.ElementAdded -= OnElementAdded;
            }
            FusionMenu.Closed -= OnMenuClosed;
        }

        private void OnElementAdded(ElementData element)
        {
            if (element == _targetElement)
            {
                _pending = true;
            }
        }

        private void OnMenuClosed()
        {
            if (!_pending)
            {
                return;
            }

            StoryTrigger.Fire(_triggerId);
            _pending = false;
        }
    }
}
