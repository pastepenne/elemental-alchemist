using System.Collections.Generic;
using ElementalAlchemist.Dialogue;
using ElementalAlchemist.GameInput;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.Characters
{
    /// <summary>Guide NPC brain. Reacts to story sequences (walk to waypoints, deliver dialogue) and shows the
    /// form matching progression. Whether he exists in a scene is gated separately by <see cref="DespawnIfStoryDone"/>.</summary>
    [RequireComponent(typeof(NpcMovement))]
    [RequireComponent(typeof(GrandMasterForms))]
    public class GrandMaster : MonoBehaviour
    {
        [SerializeField] private float _followCatchUpDistance = 10f;
        [SerializeField] private float _followStopDistance = 5f;

        [SerializeField] private string _spawnKey;
        [SerializeField] private List<NamedWaypoint> _waypoints = new();

        private NpcMovement _movement;
        private GrandMasterForms _forms;
        private DialogueData _pendingDialogue;
        private bool _waitingForArrival;
        private Transform _player;

        private void Awake()
        {
            _movement = GetComponent<NpcMovement>();
            _forms = GetComponent<GrandMasterForms>();
        }

        private void Start()
        {
            if (PlayerManager.Instance)
            {
                _player = PlayerManager.Instance.transform;
            }

            if (TryGetWaypoint(_spawnKey, out var spawnPoint))
            {
                _movement.Warp(spawnPoint.position);
            }

            _forms.ApplyCurrentForm();
        }

        private bool IsTutorialActive => ProgressionManager.Instance &&
                                         ProgressionManager.Instance.CurrentStage == ProgressionStage.Tutorial;

        private void OnEnable()
        {
            StoryDirector.SequenceStarted += OnSequenceStarted;
            StoryDirector.StepCompleted += OnStepCompleted;
            StoryDirector.SequenceCompleted += OnSequenceCompleted;
        }

        private void OnDisable()
        {
            StoryDirector.SequenceStarted -= OnSequenceStarted;
            StoryDirector.StepCompleted -= OnStepCompleted;
            StoryDirector.SequenceCompleted -= OnSequenceCompleted;
        }

        private void Update()
        {
            if (_waitingForArrival)
            {
                if (_movement.HasArrived)
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
                _movement.MoveTo(target);
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
            GoTo(step.WaypointKey, step.Dialogue, step.WarpWaypointKey);
        }

        private void OnSequenceCompleted(string sequenceId)
        {
            // A fragment may have just been granted, so update to the matching form.
            _forms.ApplyCurrentForm();
        }

        private void GoTo(string waypointKey, DialogueData dialogue, string approachWaypointKey = null)
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

            // Optional dramatic entrance: warp out of sight near the destination, then walk the last steps in.
            if (TryGetWaypoint(approachWaypointKey, out var approach))
            {
                _movement.Warp(approach.position);
            }

            ActionMapController.SetActionMap(ActionMaps.Cutscene);
            _movement.MoveTo(waypoint.position);
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
