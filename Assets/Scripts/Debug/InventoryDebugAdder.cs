using ElementalAlchemist.Data;
using ElementalAlchemist.Player;
using UnityEngine;

namespace ElementalAlchemist.Debug
{
    public class InventoryDebugAdder : MonoBehaviour
    {
        [SerializeField] private Element _airElement;
        [SerializeField] private Element _earthElement;
        [SerializeField] private Element _fireElement;
        [SerializeField] private Element _waterElement;

        public void AddAir() => AddElement(_airElement);
        public void AddEarth() => AddElement(_earthElement); 
        public void AddFire() => AddElement(_fireElement);
        public void AddWater() => AddElement(_waterElement);

        private static void AddElement(Element element)
        {
            PlayerManager.Instance.Inventory.AddElement(element, 1);
            
            UnityEngine.Debug.Log(PlayerManager.Instance.Inventory.GetStacks().Count);
        }
    }
}