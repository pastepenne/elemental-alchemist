using Constants;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private PlayerMovement _playerMovement;
    private int _speedHash;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
        
        // Cache the animator parameter hash for better performance
        _speedHash = Animator.StringToHash(AnimatorParameters.Speed);
    }
    
    private void Update()
    {
        // Normalize current speed for animator parameter (0-1 range for animation blending)
        var normalizedSpeed = _playerMovement.CurrentSpeed / _playerMovement.MaxSpeed;
        _animator.SetFloat(_speedHash, normalizedSpeed);
    }
}
