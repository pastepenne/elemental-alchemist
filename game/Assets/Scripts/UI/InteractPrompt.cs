using TMPro;
using UnityEngine;

namespace ElementalAlchemist.UI
{
    public class InteractPrompt : MonoBehaviour
    {
        [SerializeField] private float _heightOffset = 1.5f;
        [SerializeField] private float _towardCameraOffset = 1f;
        [SerializeField] private TMP_Text _promptText;

        private Transform _currentTarget;
        private Camera _camera;

        // Awake is called when the script instance is being loaded (before Start)
        private void Awake()
        {
            Hide();
        }

        // LateUpdate is called once per frame, after all Update functions have been called
        private void LateUpdate()
        {
            if (_currentTarget)
            {
                transform.position = ComputePosition();
            }
        }

        public void Show(Transform target, string promptText = "Interact")
        {
            if (!target)
            {
                return;
            }

            _promptText.text = $"[E] {promptText}";
            _currentTarget = target;
            transform.position = ComputePosition();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _promptText.text = "[E] Interact";
            _currentTarget = null;
            gameObject.SetActive(false);
        }

        // Sit above the target and biased toward the camera, so "in front" is the viewer's side
        // regardless of how the scene or object is oriented.
        private Vector3 ComputePosition()
        {
            // Re-fetch when null (the persistent prompt outlives each scene's camera).
            if (!_camera)
            {
                _camera = Camera.main;
            }

            var position = _currentTarget.position + Vector3.up * _heightOffset;
            if (_camera)
            {
                var toCamera = _camera.transform.position - _currentTarget.position;
                toCamera.y = 0f;
                if (toCamera.sqrMagnitude > 0.0001f)
                {
                    position += toCamera.normalized * _towardCameraOffset;
                }
            }

            return position;
        }
    }
}
