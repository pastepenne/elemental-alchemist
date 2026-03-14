using System;

namespace ElementalAlchemist.Data
{
    [Serializable]
    public class ElementStack
    {
        public Element element;
        public int quantity;

        public ElementStack(Element element, int quantity)
        {
            this.element = element;
            this.quantity = quantity;
        }
    }
}