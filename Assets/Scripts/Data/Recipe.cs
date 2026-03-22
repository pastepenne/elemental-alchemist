using UnityEngine;

namespace ElementalAlchemist.Data
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Elemental Alchemist/Recipe")]
    public class Recipe : ScriptableObject
    {
        public Element inputA;
        public Element inputB;
        public Element output;
    }
}
