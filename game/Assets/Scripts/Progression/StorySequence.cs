using ElementalAlchemist.Dialogue;
using UnityEngine;

namespace ElementalAlchemist.Progression
{
    [CreateAssetMenu(fileName = "New Story Sequence", menuName = "Elemental Alchemist/Story Sequence")]
    public class StorySequence : ScriptableObject
    {
        [SerializeField] private string _sequenceId;
        [SerializeField] private ProgressionStage _activationStage;
        [SerializeField] private SequenceOutcome _onComplete;
        [SerializeField] private string _openingWaypointKey;
        [SerializeField] private DialogueData _openingDialogue;
        [SerializeField] private StoryStep[] _steps;

        public string SequenceId => _sequenceId;
        public ProgressionStage ActivationStage => _activationStage;
        public SequenceOutcome OnComplete => _onComplete;
        public string OpeningWaypointKey => _openingWaypointKey;
        public DialogueData OpeningDialogue => _openingDialogue;
        public StoryStep[] Steps => _steps;
    }
}
