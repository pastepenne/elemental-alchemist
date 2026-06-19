using ElementalAlchemist.Element;
using ElementalAlchemist.Shared;
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
        public bool IsDynamic { get; private set; }

        /// <summary>Creates an in-memory recipe for a server-generated fusion, mirroring ElementData.CreateRuntime.</summary>
        public static RecipeData CreateRuntime(ElementData inputA, ElementData inputB, ElementData output)
        {
            var recipe = CreateInstance<RecipeData>();
            recipe.IsDynamic = true;
            recipe._inputA = inputA;
            recipe._inputB = inputB;
            recipe._output = output;
            recipe.name = $"{output}_{inputA.Id}-{inputB.Id}";
            return recipe;
        }

        /// <summary>Builds the serializable definition (input ids + result id) for this recipe.</summary>
        public RecipeDefinition ToDefinition() => new()
        {
            ElementA = _inputA.Id,
            ElementB = _inputB.Id,
            Result = _output.Id
        };
    }
}
