using UnityEngine;

namespace ElementalAlchemist.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        public static string PendingSpawnId;

        [SerializeField] private string _spawnId;

        private void Start()
        {
            if (!ShouldSnap())
            {
                return;
            }

            var player = GameObject.FindWithTag("Player");
            if (!player)
            {
                return;
            }

            var cc = player.GetComponent<CharacterController>();
            if (cc)
            {
                cc.enabled = false;
            }

            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;

            if (cc)
            {
                cc.enabled = true;
            }

            PendingSpawnId = null;
        }

        private bool ShouldSnap()
        {
            if (!string.IsNullOrEmpty(PendingSpawnId))
            {
                return _spawnId == PendingSpawnId;
            }

            return string.IsNullOrEmpty(_spawnId);
        }
    }
}
