using UnityEngine;
using UnityEngine.AI;

namespace ElementalAlchemist.Characters
{
    /// <summary>Generic NavMeshAgent wrapper: issues destinations and reports arrival/speed for any NPC.</summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NpcMovement : MonoBehaviour
    {
        [SerializeField] private float _arrivalDistance = 1.5f;

        private NavMeshAgent _agent;

        public float CurrentSpeed => _agent.velocity.magnitude;
        public float MaxSpeed => _agent.speed;
        public bool HasArrived => !_agent.pathPending && _agent.remainingDistance <= _arrivalDistance;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void MoveTo(Vector3 destination)
        {
            _agent.SetDestination(destination);
        }

        /// <summary>Teleport onto the NavMesh without leaving a trail (use for spawn placement).</summary>
        public void Warp(Vector3 position)
        {
            _agent.Warp(position);
        }

        public void Stop()
        {
            if (_agent.isOnNavMesh)
            {
                _agent.ResetPath();
            }
        }
    }
}
