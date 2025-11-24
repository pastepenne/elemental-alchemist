using System;

[Serializable]
public class InventorySlot
{
    public Element element;
    public int quantity;
    
    public InventorySlot(Element element, int quantity)
    {
        this.element = element;
        this.quantity = quantity;
    }
}