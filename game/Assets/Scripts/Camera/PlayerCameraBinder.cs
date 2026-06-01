using ElementalAlchemist.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace ElementalAlchemist.CameraSystem
{
    /// <summary>Binds the scene's Cinemachine camera to the persistent player on scene load.</summary>
    [RequireComponent(typeof(CinemachineCamera))]
    public class PlayerCameraBinder : MonoBehaviour
    {
        [SerializeField] private bool _useFollow = true;
        [SerializeField] private bool _useLookAt = true;

        private void Start()
        {
            if (!PlayerManager.Instance)
            {
                return;
            }

            var vcam = GetComponent<CinemachineCamera>();
            var target = PlayerManager.Instance.transform;

            if (_useFollow)
            {
                vcam.Follow = target;
            }

            if (_useLookAt)
            {
                vcam.LookAt = target;
            }
        }
    }
}
