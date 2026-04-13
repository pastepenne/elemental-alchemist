using ElementalAlchemist.Element;
using UnityEngine;

namespace ElementalAlchemist.Fusion
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Elemental Alchemist/Recipe")]
    public class RecipeData : ScriptableObject
    {
        [SerializeField] private ElementData _inputA;
        [SerializeField] private ElementData _inputB;
        [SerializeField] private ElementData _output;
        
        public ElementData inputA => _inputA;
        public ElementData inputB => _inputB;
        public ElementData output => _output;
    }
}
