using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElementalAlchemist.Progression
{
    /// <summary>Singleton that runs the active StorySequence based on the current progression stage.</summary>
    public class StoryDirector : MonoBehaviour
    {
        public static StoryDirector Instance { get; private set; }

        public static event Action<StorySequence> SequenceStarted;
        public static event Action<StoryStep> StepStarted;
        public static event Action<StoryStep> StepCompleted;
        public static event Action<string> SequenceCompleted;

        [SerializeField] private StorySequence[] _sequences;

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

        private void Start()
        {
            StoryTrigger.Fired += OnTriggerFired;

            if (ProgressionManager.Instance)
            {
                TryStartSequenceForStage(ProgressionManager.Instance.CurrentStage);
            }
        }

        private void OnDestroy()
        {
            StoryTrigger.Fired -= OnTriggerFired;
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void TryStartSequenceForStage(ProgressionStage stage)
        {
            var sequence = FindSequence(stage);
            if (!sequence)
            {
                _currentSequence = null;
                _currentStep = null;
                return;
            }

            var startIndex = _stepIndexBySequenceId.TryGetValue(sequence.SequenceId, out var stored) ? stored : 0;
            if (startIndex >= sequence.Steps.Length)
            {
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

        private StorySequence FindSequence(ProgressionStage stage)
        {
            foreach (var sequence in _sequences)
            {
                if (sequence && sequence.ActivationStage == stage && sequence.Steps != null && sequence.Steps.Length > 0)
                {
                    return sequence;
                }
            }
            return null;
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

                if (ProgressionManager.Instance)
                {
                    TryStartSequenceForStage(ProgressionManager.Instance.CurrentStage);
                }
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
