using System.Collections.Generic;
using System.Linq;
using ElementalAlchemist.Element;
using UnityEngine;

namespace ElementalAlchemist.Fusion
{
    [CreateAssetMenu(fileName = "New Recipe Catalog", menuName = "Elemental Alchemist/Recipe Catalog")]
    public class RecipeCatalog : ScriptableObject
    {
        [SerializeField] private RecipeData[] _recipes;

        private Dictionary<RecipeKey, RecipeData> _lookup;

        private void OnEnable()
        {
            _lookup = new Dictionary<RecipeKey, RecipeData>();

            if (_recipes == null)
            {
                return;
            }

            foreach (var recipe in _recipes)
            {
                if (!recipe || !recipe.inputA || !recipe.inputB)
                {
                    continue;
                }

                var key = new RecipeKey(recipe.inputA.Id, recipe.inputB.Id);
                _lookup.TryAdd(key, recipe);
            }
        }

        public RecipeData FindRecipe(ElementData a, ElementData b)
        {
            if (_lookup == null)
            {   
                return null;
            }
            
            if (!a || !b)
            {
                return null;
            }
            
            var key = new RecipeKey(a.Id, b.Id);
            _lookup.TryGetValue(key, out var recipe);
            return recipe;
        }
        
        public List<RecipeData> GetRecipesForOutput(ElementData element)
        {
            return _recipes != null 
                ? _recipes.Where(r => r && r.output == element).ToList()
                : new List<RecipeData>();
        }
    }
}
