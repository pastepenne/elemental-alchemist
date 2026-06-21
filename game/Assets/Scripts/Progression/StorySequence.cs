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
        [SerializeField] private string _openingWarpWaypointKey;
        [SerializeField] private DialogueData _openingDialogue;
        [SerializeField] private StoryStep[] _steps;

        public string SequenceId => _sequenceId;
        public ProgressionStage ActivationStage => _activationStage;
        public SequenceOutcome OnComplete => _onComplete;
        public string OpeningWaypointKey => _openingWaypointKey;

        /// <summary>Optional off-screen point the actor warps to before walking the opening in, for a dramatic entrance.</summary>
        public string OpeningWarpWaypointKey => _openingWarpWaypointKey;
        public DialogueData OpeningDialogue => _openingDialogue;
        public StoryStep[] Steps => _steps;
    }
}
