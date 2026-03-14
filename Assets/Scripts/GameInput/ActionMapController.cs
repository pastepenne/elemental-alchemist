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

            InputSystem.actions.FindActionMap(_current).Disable();
            _current = actionMap;
            InputSystem.actions.FindActionMap(_current).Enable();
        }
    }
}