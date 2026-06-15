using UnityEngine;

namespace ElementalAlchemist.Progression
{
    /// <summary>Destroys this object at scene load if its story trigger has already fired - e.g. a one-time
    /// cutscene NPC that should not reappear once its moment has passed.</summary>
    public class DespawnIfStoryDone : MonoBehaviour
    {
        [SerializeField] private string _triggerId;

        // Start (not Awake) so the persistent StoryDirector singleton is guaranteed to exist.
        private void Start()
        {
            if (StoryDirector.Instance && StoryDirector.Instance.HasFired(_triggerId))
            {
                Destroy(gameObject);
            }
        }
    }
}
