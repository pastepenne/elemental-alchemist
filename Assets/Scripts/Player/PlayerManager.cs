using UnityEngine;

namespace ElementalAlchemist.Player
{
    /// <summary>
    /// Singleton entry point for all player systems. Persists across scene loads.
    /// </summary>
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
            // Ensure only one instance of PlayerManager exists
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                Discovery = new Discovery();
                Inventory = new Inventory();
                Inventory.ElementAdded += Discovery.DiscoverElement;

                Animation = GetComponent<PlayerAnimation>();
                Interaction = GetComponent<PlayerInteraction>();
                Movement = GetComponent<PlayerMovement>();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}