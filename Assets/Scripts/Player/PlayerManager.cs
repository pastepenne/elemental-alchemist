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
        public PlayerInteraction Interaction { get; private set; }
        public PlayerInventory Inventory { get; private set; }
        public PlayerMovement Movement { get; private set; }

        private void Awake()
        {
            // Ensure only one instance of PlayerManager exists
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                Animation = GetComponent<PlayerAnimation>();
                Interaction = GetComponent<PlayerInteraction>();
                Inventory = new PlayerInventory();
                Movement = GetComponent<PlayerMovement>();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}