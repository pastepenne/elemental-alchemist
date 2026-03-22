using UnityEngine;

namespace ElementalAlchemist.UI
{
    public class BillboardCanvas : MonoBehaviour
    {
        private Camera _mainCamera;

        private void OnEnable()
        {
            _mainCamera = Camera.main;
        }

        // LateUpdate is called once per frame, after all Update functions have been called
        private void LateUpdate()
        {
            // Skip if no camera is available
            if (!_mainCamera)
            {
                return;
            }
        
            // Make the canvas face the camera
            transform.LookAt(transform.position + _mainCamera.transform.forward);
        }
    }
}