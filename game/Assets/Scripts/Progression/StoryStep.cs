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
        [SerializeField] private string _approachWaypointKey;
        [SerializeField] private DialogueData _dialogue;

        public string TriggerId => _triggerId;
        public string ObjectiveText => _objectiveText;
        public string WaypointKey => _waypointKey;

        public string WarpWaypointKey => _approachWaypointKey;
        public DialogueData Dialogue => _dialogue;
    }
}
