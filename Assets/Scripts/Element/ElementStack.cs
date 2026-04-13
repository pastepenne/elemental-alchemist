using System;

namespace ElementalAlchemist.Element
{
    public class ElementStack
    {
        public ElementData Element { get; private set; }
        public int Quantity { get; set; }

        public ElementStack(ElementData element, int quantity)
        {
            Element = element;
            Quantity = quantity;
        }
    }
}