using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElementalAlchemist.Progression
{
    /// <summary>Singleton that runs one StorySequence at a time. Scenes begin their sequence via
    /// <see cref="StorySequenceStarter"/>; the director ignores a sequence that is already running or finished.</summary>
    public class StoryDirector : MonoBehaviour
    {
        public static StoryDirector Instance { get; private set; }

        public static event Action<StorySequence> SequenceStarted;
        public static event Action<StoryStep> StepStarted;
        public static event Action<StoryStep> StepCompleted;
        public static event Action<string> SequenceCompleted;

        private readonly Dictionary<string, int> _stepIndexBySequenceId = new();
        private readonly HashSet<string> _firedTriggers = new();

        private StorySequence _currentSequence;
        private int _currentIndex;
        private StoryStep _currentStep;

        public StorySequence CurrentSequence => _currentSequence;
        public StoryStep CurrentStep => _currentStep;

        public bool IsCurrentTrigger(string triggerId) => _currentStep && _currentStep.TriggerId == triggerId;
        public bool HasFired(string triggerId) => _firedTriggers.Contains(triggerId);

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            StoryTrigger.Fired += OnTriggerFired;
        }

        private void OnDisable()
        {
            StoryTrigger.Fired -= OnTriggerFired;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>Begins a sequence, or resumes it if the scene is re-entered. Does nothing if it is already
        /// running or already finished.</summary>
        public void BeginSequence(StorySequence sequence)
        {
            if (!sequence || sequence.Steps == null || sequence.Steps.Length == 0)
            {
                return;
            }

            // Already running this sequence: re-emit the current step so a freshly loaded ObjectiveHud repopulates,
            // but never replay the opening.
            if (_currentSequence == sequence)
            {
                if (_currentStep)
                {
                    StepStarted?.Invoke(_currentStep);
                }
                return;
            }

            var startIndex = _stepIndexBySequenceId.TryGetValue(sequence.SequenceId, out var stored) ? stored : 0;
            if (startIndex >= sequence.Steps.Length)
            {
                // Sequence already completed in a previous visit.
                return;
            }

            _currentSequence = sequence;
            _currentIndex = startIndex;

            if (startIndex == 0)
            {
                SequenceStarted?.Invoke(sequence);
            }

            StartStep(sequence.Steps[_currentIndex]);
        }

        private void StartStep(StoryStep step)
        {
            _currentStep = step;
            StepStarted?.Invoke(step);
        }

        private void OnTriggerFired(string id)
        {
            if (!_currentStep || id != _currentStep.TriggerId)
            {
                return;
            }

            _firedTriggers.Add(id);

            var finished = _currentStep;
            _currentStep = null;
            StepCompleted?.Invoke(finished);

            _currentIndex++;
            _stepIndexBySequenceId[_currentSequence.SequenceId] = _currentIndex;

            if (_currentIndex >= _currentSequence.Steps.Length)
            {
                var finishedSequence = _currentSequence;
                _currentSequence = null;
                ApplyOutcome(finishedSequence.OnComplete);
                SequenceCompleted?.Invoke(finishedSequence.SequenceId);
                return;
            }

            StartStep(_currentSequence.Steps[_currentIndex]);
        }

        private static void ApplyOutcome(SequenceOutcome outcome)
        {
            if (!ProgressionManager.Instance)
            {
                return;
            }

            switch (outcome)
            {
                case SequenceOutcome.CompleteTutorial:
                    ProgressionManager.Instance.OnTutorialCompleted();
                    break;
                case SequenceOutcome.GrantBreathFragment:
                    ProgressionManager.Instance.OnBreathFragmentGranted();
                    break;
                case SequenceOutcome.GrantFleshFragment:
                    ProgressionManager.Instance.OnFleshFragmentGranted();
                    break;
                case SequenceOutcome.GrantSoulFragment:
                    ProgressionManager.Instance.OnSoulFragmentGranted();
                    break;
            }
        }
    }
}
