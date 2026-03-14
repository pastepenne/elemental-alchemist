using UnityEngine;

namespace ElementalAlchemist.Player
{
    /// <summary>
    /// Singleton entry point for all player systems. Persists across scene loads.
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        public PlayerMovement PlayerMovement { get; private set; }
        public PlayerAnimation PlayerAnimation { get; private set; }
        public PlayerInventory PlayerInventory { get; private set; }

        private void Awake()
        {
            // Ensure only one instance of PlayerManager exists
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                PlayerMovement = GetComponent<PlayerMovement>();
                PlayerAnimation = GetComponent<PlayerAnimation>();
                PlayerInventory = new PlayerInventory();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}