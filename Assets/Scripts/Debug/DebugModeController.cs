using ElementalAlchemist.GameInput;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugModeController : MonoBehaviour
{
    [SerializeField] [CanBeNull] private GameObject _debugPanel;
    
    private InputAction _debugAction;

    private void Awake()
    {
        _debugAction = InputSystem.actions.FindAction(InputActions.Global.Debug);

        // Hide debug panel on start
        if (_debugPanel != null)
        {
            _debugPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        _debugAction.performed += OnToggleDebug;
    }

    private void OnDisable()
    {
        _debugAction.performed -= OnToggleDebug;
    }

    private void OnToggleDebug(InputAction.CallbackContext context)
    {
        if (_debugPanel == null)
        {
            return;
        }
        
        _debugPanel.SetActive(!_debugPanel.activeSelf);
    }
}
