using ElementalAlchemist.Element;
using ElementalAlchemist.GameInput;
using ElementalAlchemist.Player;
using UnityEngine;

namespace ElementalAlchemist.Progression
{
    public class InventoryStoryTrigger : MonoBehaviour
    {
        [SerializeField] private ElementData _element;
        [SerializeField, Min(1)] private int _requiredQuantity = 1;
        [SerializeField] private string _triggerId;

        private bool _armed;
        private bool _deferred;

        private void OnEnable()
        {
            StoryDirector.StepStarted += OnStepStarted;
            ActionMapController.Changed += OnControlChanged;
            if (PlayerManager.Instance)
            {
                PlayerManager.Instance.Inventory.ElementAdded += OnElementAdded;
            }
            
            if (StoryDirector.Instance)
            {
                OnStepStarted(StoryDirector.Instance.CurrentStep);
            }
        }

        private void OnDisable()
        {
            StoryDirector.StepStarted -= OnStepStarted;
            ActionMapController.Changed -= OnControlChanged;
            if (PlayerManager.Instance)
            {
                PlayerManager.Instance.Inventory.ElementAdded -= OnElementAdded;
            }
        }

        private void OnStepStarted(StoryStep step)
        {
            _armed = step && step.TriggerId == _triggerId;
            TryFire();
        }

        private void OnElementAdded(ElementData element)
        {
            if (_armed && element == _element)
            {
                TryFire();
            }
        }

        private void OnControlChanged(string map)
        {
            if (_deferred && map == ActionMaps.Player)
            {
                TryFire();
            }
        }

        private void TryFire()
        {
            if (!_armed || !PlayerManager.Instance)
            {
                return;
            }

            if (!PlayerManager.Instance.Inventory.HasElement(_element, _requiredQuantity))
            {
                return;
            }
            
            if (ActionMapController.Target != ActionMaps.Player)
            {
                _deferred = true;
                return;
            }

            _deferred = false;
            _armed = false;
            StoryTrigger.Fire(_triggerId);
        }
    }
}
