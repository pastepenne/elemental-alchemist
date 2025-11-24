using System;
using UnityEngine;

public class InventoryDebugAdder : MonoBehaviour
{
    [SerializeField] private Element _rockElement;
    [SerializeField] private Element _woodElement;

    public void AddRock()
    {
        Inventory.Instance.AddElement(_rockElement);
    }

    public void AddWood()
    {
        Inventory.Instance.AddElement(_woodElement);
    }
}