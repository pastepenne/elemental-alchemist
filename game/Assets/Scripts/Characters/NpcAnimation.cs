using ElementalAlchemist.Constants;
using UnityEngine;

namespace ElementalAlchemist.Characters
{
    /// <summary>Generic NPC Animator driver: locomotion blend from movement speed, plus one-off gesture triggers.</summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NpcMovement))]
    public class NpcAnimation : MonoBehaviour
    {
        private Animator _animator;
        private NpcMovement _movement;
        private int _speedHash;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movement = GetComponent<NpcMovement>();
            _speedHash = Animator.StringToHash(AnimatorParameters.Speed);
        }

        private void Update()
        {
            // Normalize current speed for animator blending (0-1 range)
            var normalizedSpeed = _movement.MaxSpeed > 0f ? _movement.CurrentSpeed / _movement.MaxSpeed : 0f;
            _animator.SetFloat(_speedHash, normalizedSpeed);
        }

        /// <summary>Fire a one-off animation trigger (wave, point, cast) from a story beat.</summary>
        public void PlayGesture(int triggerHash)
        {
            _animator.SetTrigger(triggerHash);
        }
    }
}
