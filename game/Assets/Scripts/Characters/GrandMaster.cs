using System.Collections.Generic;
using ElementalAlchemist.Dialogue;
using ElementalAlchemist.GameInput;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.Characters
{
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
        private bool _reportOutroToDirector;
        private Transform _player;

        private void Awake()
        {
            _movement = GetComponent<NpcMovement>();
            _forms = GetComponent<GrandMasterForms>();
        }

        private void Start()
        {
            RefreshPresence();

            if (PlayerManager.Instance)
            {
                _player = PlayerManager.Instance.transform;
            }

            // Don't clobber an opening entrance that already kicked off: SequenceStarted (handled in OnEnable, before
            // this Start) may have warped him off-screen to walk in, and re-warping to the spawn would cancel that.
            if (!_waitingForArrival && TryGetWaypoint(_spawnKey, out var spawnPoint))
            {
                _movement.Warp(spawnPoint.position);
            }

            _forms.ApplyCurrentForm();
        }

        // The master shepherds the player by trailing them: through the Tutorial, and again in the endgame once the
        // Soul fragment is won (until freeplay begins), so he is already at the Sanctuary for the finale.
        private bool ShouldFollowPlayer
        {
            get
            {
                var progression = ProgressionManager.Instance;
                if (!progression)
                {
                    return false;
                }

                return progression.CurrentStage is ProgressionStage.Tutorial or ProgressionStage.Master;
            }
        }

        private void OnEnable()
        {
            StoryDirector.SequenceStarted += OnSequenceStarted;
            StoryDirector.StepCompleted += OnStepCompleted;
            StoryDirector.SequenceCompleted += OnSequenceCompleted;
            DialogueManager.DialogueEnded += OnDialogueEnded;
        }

        private void OnDisable()
        {
            StoryDirector.SequenceStarted -= OnSequenceStarted;
            StoryDirector.StepCompleted -= OnStepCompleted;
            StoryDirector.SequenceCompleted -= OnSequenceCompleted;
            DialogueManager.DialogueEnded -= OnDialogueEnded;
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
            if (!_player || !ShouldFollowPlayer)
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

        private void RefreshPresence()
        {
            if (ProgressionManager.Instance && ProgressionManager.Instance.IsFreeplayActive)
            {
                Destroy(gameObject);
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
                ReportStepOutroFinished(); // Arrived with no line to speak: the outro is already over.
            }
            _pendingDialogue = null;
        }

        private void OnSequenceStarted(StorySequence sequence)
        {
            _reportOutroToDirector = false; // The opening is not a step outro.
            GoTo(sequence.OpeningWaypointKey, sequence.OpeningDialogue, sequence.OpeningWarpWaypointKey);
        }

        private void OnStepCompleted(StoryStep step)
        {
            // The Guardian grants the fragment before firing this step's trigger, so progression is already current.
            // Refresh the form now, before he sets off, so he approaches the player already wearing the new one.
            _forms.ApplyCurrentForm();

            _reportOutroToDirector = !string.IsNullOrEmpty(step.WaypointKey) || step.Dialogue;
            GoTo(step.WaypointKey, step.Dialogue, step.WarpWaypointKey);
        }

        private void OnSequenceCompleted(string sequenceId)
        {
            RefreshPresence();
        }

        private void OnDialogueEnded()
        {
            // When a step's outro dialogue finishes, the sequence can complete.
            ReportStepOutroFinished();
        }

        private void ReportStepOutroFinished()
        {
            if (!_reportOutroToDirector)
            {
                return;
            }

            _reportOutroToDirector = false;

            // The director no-ops this unless it is the final step's outro; it owns applying the sequence outcome.
            if (StoryDirector.Instance)
            {
                StoryDirector.Instance.NotifyStepOutroFinished();
            }
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
