using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    
    [SerializeField] private List<InventorySlot> _slots = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddElement(Element element, int quantity = 1)
    {
        if (element == null || quantity <= 0)
        {
            return;
        }

        var matchingSlot = _slots.FirstOrDefault(slot => slot.element == element);
        if (matchingSlot != null)
        {
            matchingSlot.quantity += quantity;
        }
        else
        {
            _slots.Add(new(element, quantity));
        }
    }
}
