using System;
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

        public static string Current => _current;
        public static event Action<string> Changed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            _current = ActionMaps.Player;
            InputSystem.actions.FindActionMap(ActionMaps.Global).Enable();
        }

        /// <summary>Forces input back to the default gameplay map. Call when (re)entering from the menu, since
        /// _current is app-lifetime static state that Init only seeds once at startup.</summary>
        public static void Reset()
        {
            if (_pending != null)
            {
                InputSystem.onAfterUpdate -= ApplyPendingSwitch;
                _pending = null;
            }

            if (_current != null)
            {
                InputSystem.actions.FindActionMap(_current).Disable();
            }

            _current = ActionMaps.Player;
            InputSystem.actions.FindActionMap(_current).Enable();
            Changed?.Invoke(_current);
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
            Changed?.Invoke(_current);
        }
    }
}