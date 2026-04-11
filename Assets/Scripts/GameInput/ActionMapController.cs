using UnityEngine;
using UnityEngine.InputSystem;

namespace ElementalAlchemist.GameInput
{
    /// <summary>
    /// Manages the currently active action map.
    /// </summary>
    public static class ActionMapController
    {
        private static string _current;
        private static string _pending;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            _current = ActionMaps.Player;
            InputSystem.actions.FindActionMap(ActionMaps.Global).Enable();
        }

        public static void SetActionMap(string actionMap)
        {
            if (_current == actionMap)
            {
                return;
            }
            
            if (_pending == null)
            {
                InputSystem.onAfterUpdate += ApplyPendingSwitch;
            }
            
            _pending = actionMap;
        }
        
        private static void ApplyPendingSwitch()
        {
            InputSystem.onAfterUpdate -= ApplyPendingSwitch;
            InputSystem.actions.FindActionMap(_current).Disable();
            _current = _pending;
            _pending = null;
            InputSystem.actions.FindActionMap(_current).Enable();
        }
    }
}