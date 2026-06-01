using System.Collections.Generic;
using ElementalAlchemist.Dialogue;
using ElementalAlchemist.GameInput;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using UnityEngine;
using UnityEngine.AI;

namespace ElementalAlchemist.Characters
{
    /// <summary>Guide NPC. Walks to the opening waypoint at sequence start and to each step's waypoint on completion.
    /// When idle, can casually follow the player to stay nearby.</summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mentor : MonoBehaviour
    {
        [SerializeField] private float _followCatchUpDistance = 10f;
        [SerializeField] private float _followStopDistance = 5f;

        [SerializeField] private float _arrivalDistance = 1.5f;
        [SerializeField] private string _spawnKey;
        [SerializeField] private List<NamedWaypoint> _waypoints = new();

        private NavMeshAgent _agent;
        private DialogueData _pendingDialogue;
        private bool _waitingForArrival;
        private Transform _player;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            if (PlayerManager.Instance)
            {
                _player = PlayerManager.Instance.transform;
            }

            if (TryGetWaypoint(_spawnKey, out var spawnPoint))
            {
                transform.position = spawnPoint.position;
            }
        }

        private bool IsTutorialActive => ProgressionManager.Instance &&
                                         ProgressionManager.Instance.CurrentStage == ProgressionStage.Tutorial;

        private void OnEnable()
        {
            StoryDirector.SequenceStarted += OnSequenceStarted;
            StoryDirector.StepCompleted += OnStepCompleted;
        }

        private void OnDisable()
        {
            StoryDirector.SequenceStarted -= OnSequenceStarted;
            StoryDirector.StepCompleted -= OnStepCompleted;
        }

        private void Update()
        {
            if (_waitingForArrival)
            {
                if (!_agent.pathPending && _agent.remainingDistance <= _arrivalDistance)
                {
                    _waitingForArrival = false;
                    OnArrived();
                }
                return;
            }

            FollowPlayerIfNeeded();
        }

        private void FollowPlayerIfNeeded()
        {
            if (!_player || !IsTutorialActive)
            {
                return;
            }

            var distance = Vector3.Distance(transform.position, _player.position);
            if (distance > _followCatchUpDistance)
            {
                var direction = (_player.position - transform.position).normalized;
                var target = _player.position - direction * _followStopDistance;
                _agent.SetDestination(target);
            }
        }

        private void OnArrived()
        {
            if (_pendingDialogue)
            {
                DialogueManager.Instance.StartDialogue(_pendingDialogue);
            }
            else
            {
                ActionMapController.SetActionMap(ActionMaps.Player);
            }
            _pendingDialogue = null;
        }

        private void OnSequenceStarted(StorySequence sequence)
        {
            GoTo(sequence.OpeningWaypointKey, sequence.OpeningDialogue);
        }

        private void OnStepCompleted(StoryStep step)
        {
            GoTo(step.WaypointKey, step.Dialogue);
        }

        private void GoTo(string waypointKey, DialogueData dialogue)
        {
            _pendingDialogue = dialogue;

            if (!TryGetWaypoint(waypointKey, out var waypoint))
            {
                if (dialogue)
                {
                    DialogueManager.Instance.StartDialogue(dialogue);
                }
                _pendingDialogue = null;
                return;
            }

            ActionMapController.SetActionMap(ActionMaps.Cutscene);
            _agent.SetDestination(waypoint.position);
            _waitingForArrival = true;
        }

        private bool TryGetWaypoint(string key, out Transform result)
        {
            result = null;
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            foreach (var entry in _waypoints)
            {
                if (entry.Key == key && entry.Transform)
                {
                    result = entry.Transform;
                    return true;
                }
            }

            return false;
        }

        [System.Serializable]
        public class NamedWaypoint
        {
            [SerializeField] private string _key;
            [SerializeField] private Transform _transform;

            public string Key => _key;
            public Transform Transform => _transform;
        }
    }
}
