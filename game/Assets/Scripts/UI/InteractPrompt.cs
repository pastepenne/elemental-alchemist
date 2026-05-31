using TMPro;
using UnityEngine;

namespace ElementalAlchemist.UI
{
    public class InteractPrompt : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset = new(0f, 0.25f, 0f);
        [SerializeField] private TMP_Text _promptText;

        private Transform _currentTarget;

        // Awake is called when the script instance is being loaded (before Start)
        private void Awake()
        {
            Hide();
        }

        // LateUpdate is called once per frame, after all Update functions have been called
        private void LateUpdate()
        {
            // Follow the current target if one is set
            if (_currentTarget)
            {
                transform.position = _currentTarget.position + _offset;
            }
        }
    
        public void Show(Transform target, string promptText = "Interact")
        {
            if (!target)
            {
                return;
            }

            _promptText.text = promptText;
            _currentTarget = target;
            transform.position = _currentTarget.position + _offset;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _promptText.text = "Interact";
            _currentTarget = null;
            gameObject.SetActive(false);
        }
    }
}