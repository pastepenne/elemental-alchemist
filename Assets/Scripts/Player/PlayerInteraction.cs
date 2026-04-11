using ElementalAlchemist.GameInput;
using ElementalAlchemist.Interaction;
using ElementalAlchemist.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ElementalAlchemist.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float _interactDistance = 2.5f;
        [SerializeField] private LayerMask _interactableLayers;
        [SerializeField] private InteractPrompt _interactPrompt;

        private CharacterController _characterController;
        private InputAction _interactAction;

        private IInteractable _currentInteractable;

        // Awake is called when the script instance is being loaded (before Start)
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _interactAction = InputSystem.actions.FindAction(InputActions.Player.Interact);
        }

        // Update is called once per frame
        private void Update()
        {
            // build capsule from character controller
            var radius = _characterController.radius;
            var height = Mathf.Max(_characterController.height, radius * 2f);

            var center = transform.TransformPoint(_characterController.center);
            var bottom = center + Vector3.down * (height / 2f - radius);
            var top = center + Vector3.up * (height / 2f - radius);
            var direction = transform.forward;

            IInteractable foundSource = null;
            if (Physics.CapsuleCast(
                    point1: bottom,
                    point2: top,
                    radius: radius,
                    direction: direction,
                    hitInfo: out var hit,
                    maxDistance: _interactDistance,
                    layerMask: _interactableLayers))
            {
                var source = hit.collider.GetComponentInParent<IInteractable>();
                if (source != null)
                {
                    _interactPrompt.Show(hit.transform, source.Prompt);
                    foundSource = source;
                }
            }

            _currentInteractable = foundSource;

            // Hide prompt if no interactable object was found
            if (_currentInteractable == null)
            {
                _interactPrompt.Hide();
            }
        }

        private void OnEnable()
        {
            _interactAction.started += OnInteract;
        }

        private void OnDisable()
        {
            _interactAction.started -= OnInteract;

            // Check if game object still exists, before hiding
            if (_interactPrompt && _interactPrompt.gameObject)
            {
                _interactPrompt.Hide();
            }
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            // Only interact if we currently see an item source
            _currentInteractable?.Interact();
        }
    }
}