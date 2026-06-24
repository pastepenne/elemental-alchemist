using ElementalAlchemist.Dialogue;
using UnityEngine;

namespace ElementalAlchemist.Progression
{
    [CreateAssetMenu(fileName = "New Story Step", menuName = "Elemental Alchemist/Story Step")]
    public class StoryStep : ScriptableObject
    {
        [SerializeField] private string _triggerId;
        [SerializeField] private string _objectiveText;
        [SerializeField] private string _waypointKey;
        [SerializeField] private string _warpWaypointKey;
        [SerializeField] private bool _approachPlayer;
        [SerializeField] private DialogueData _dialogue;

        public string TriggerId => _triggerId;
        public string ObjectiveText => _objectiveText;
        public string WaypointKey => _waypointKey;

        public string WarpWaypointKey => _warpWaypointKey;

        /// <summary>When set, the actor delivers this step's outro by walking up to the player instead of a fixed
        /// waypoint - use when the completion spot is unpredictable (e.g. pickups scattered around the level).</summary>
        public bool ApproachPlayer => _approachPlayer;
        public DialogueData Dialogue => _dialogue;
    }
}
