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

            // Match the authored recipe filename scheme: output=inputA+inputB, inputs ordinal-sorted
            // so the name is order-independent (mirrors RecipeKey).
            var a = inputA.Id;
            var b = inputB.Id;
            if (string.CompareOrdinal(a, b) > 0)
            {
                (a, b) = (b, a);
            }
            recipe.name = $"{output.Id}={a}+{b}";
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
