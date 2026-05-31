using ElementalAlchemist.GameInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ElementalAlchemist.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _baseSpeed = 5f;
        [SerializeField] private float _sprintMultiplier = 2f;
        [SerializeField] private float _rotationSpeed = 720f;
    
        private CharacterController _characterController;
        private InputAction _moveAction;
        private InputAction _sprintAction;
        private Vector2 _moveInput;
        private float _verticalVelocity;
        private bool _isSprinting;
        private Camera _camera;
        
        // Current player speed
        public float CurrentSpeed => _moveInput.magnitude * GetSpeedMultiplier();
        // Maximum possible speed
        public float MaxSpeed => _baseSpeed * _sprintMultiplier;
    
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _moveAction = InputSystem.actions.FindAction(InputActions.Player.Move);
            _sprintAction = InputSystem.actions.FindAction(InputActions.Player.Sprint);
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            // Subscribe to input action events (when movement starts and stops)
            _moveAction.performed += OnMove;
            _moveAction.canceled += OnMove;
        
            // Subscribe to sprint action events (when sprint starts and stops)
            _sprintAction.performed += OnSprint;
            _sprintAction.canceled += OnSprint;
        }

        private void OnDisable()
        {
            // Unsubscribe from input action events
            _moveAction.performed -= OnMove;
            _moveAction.canceled -= OnMove;
        
            // Unsubscribe from sprint action events
            _sprintAction.performed -= OnSprint;
            _sprintAction.canceled -= OnSprint;
        
            // Reset cached input to prevent movement after disabling
            _moveInput = Vector2.zero;
            _isSprinting = false;
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
        
            var movement = Vector3.zero;
            if (_moveInput != Vector2.zero)
            {
                var direction = GetCameraRelativeDirection(_moveInput);
                movement = direction * (_moveInput.magnitude * GetSpeedMultiplier());

                // Rotate character to face movement direction
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }

            movement.y = _verticalVelocity;
            _characterController.Move(movement * Time.deltaTime);
        }

        private Vector3 GetCameraRelativeDirection(Vector2 input)
        {
            if (!_camera)
            {
                return new Vector3(input.x, 0f, input.y).normalized;
            }

            // Project camera forward onto the XZ plane and normalize
            var camForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            var camRight = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;

            return (camForward * input.y + camRight * input.x).normalized;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            // Cache the movement input
            _moveInput = context.ReadValue<Vector2>();
        }
    
        private void OnSprint(InputAction.CallbackContext context)
        {
            // Check if sprint button is held down
            _isSprinting = context.ReadValueAsButton();
        }
    
        private float GetSpeedMultiplier() => _isSprinting ? _baseSpeed * _sprintMultiplier : _baseSpeed;
    }
}
