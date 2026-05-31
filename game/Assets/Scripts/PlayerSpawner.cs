using UnityEngine;

namespace ElementalAlchemist
{
    public class PlayerSpawner : MonoBehaviour
    {
        private void Start()
        {
            var player = GameObject.FindWithTag("Player");

            if (player == null)
            {
                return;
            }
            
            
            // Turn off CharacterController temporarily if you use one
            var cc = player.GetComponent<CharacterController>();
            if (cc)
            {
                cc.enabled = false;
            }

            // Snap player to this object's position and rotation
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;

            if (cc)
            {
                cc.enabled = true;
            }
        }
    }
}