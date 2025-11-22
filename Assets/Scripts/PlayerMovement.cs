using Constants;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    
    private CharacterController _characterController;
    private InputAction _moveAction;
    private Vector2 _moveInput;
    private float _verticalVelocity;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _moveAction = InputSystem.actions.FindAction(InputActions.Move);
    }

    private void OnEnable()
    {
        // Subscribe to input action events (when movement starts and stops)
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
    }

    private void OnDisable()
    {
        // Unsubscribe from input action events
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
        
        // Reset cached input to prevent movement after disabling
        _moveInput = Vector2.zero;
    }

    private void Update()
    {
        // Apply gravity
        if (!_characterController.isGrounded)
        {
            // The character falls faster over time
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            // Push character down to keep it stuck to the ground
            _verticalVelocity = -2f;
        }

        // Calculate movement
        var movement = _moveInput != Vector2.zero 
            ? new Vector3(_moveInput.x, 0f, _moveInput.y) * _moveSpeed 
            : Vector3.zero;
        
        movement.y = _verticalVelocity;

        _characterController.Move(movement * Time.deltaTime);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Cache the movement input
        _moveInput = context.ReadValue<Vector2>();
    }
}
