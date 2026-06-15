using ElementalAlchemist.Progression;
using UnityEngine;

namespace ElementalAlchemist.Player
{
    /// <summary>
    /// Singleton entry point for all player systems. Persists across scene loads.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAnimation))]
    [RequireComponent(typeof(PlayerInteraction))]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }
        public PlayerAnimation Animation { get; private set; }
        public Discovery Discovery { get; private set; }
        public PlayerInteraction Interaction { get; private set; }
        public Inventory Inventory { get; private set; }
        public PlayerMovement Movement { get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                Discovery = new Discovery();
                Inventory = new Inventory();
                Inventory.ElementAdded += Discovery.DiscoverElement;
                ProgressionManager.CoreUnlocked += Discovery.DiscoverElement;

                Animation = GetComponent<PlayerAnimation>();
                Interaction = GetComponent<PlayerInteraction>();
                Movement = GetComponent<PlayerMovement>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            foreach (var element in ProgressionManager.Instance.UnlockedCoreElements)
            {
                Discovery.DiscoverElement(element);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                ProgressionManager.CoreUnlocked -= Discovery.DiscoverElement;
            }
        }
    }
}
