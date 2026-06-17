using UnityEngine;

namespace ElementalAlchemist.Progression
{
    public class StorySequenceStarter : MonoBehaviour
    {
        [SerializeField] private StorySequence _sequence;

        private void Start()
        {
            if (StoryDirector.Instance)
            {
                StoryDirector.Instance.BeginSequence(_sequence);
            }
        }
    }
}
