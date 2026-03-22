using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ElementalAlchemist.Data
{
    [CreateAssetMenu(fileName = "New Recipe Catalog", menuName = "Elemental Alchemist/Recipe Catalog")]
    public class RecipeCatalog : ScriptableObject
    {
        [SerializeField] private Recipe[] _recipes;

        private Dictionary<RecipeKey, Recipe> _lookup;

        private void OnEnable()
        {
            _lookup = new Dictionary<RecipeKey, Recipe>();

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

                var key = new RecipeKey(recipe.inputA.id, recipe.inputB.id);
                _lookup.TryAdd(key, recipe);
            }
        }

        public Recipe FindRecipe(Element a, Element b)
        {
            if (_lookup == null)
            {   
                return null;
            }
            
            if (!a || !b)
            {
                return null;
            }
            
            var key = new RecipeKey(a.id, b.id);
            _lookup.TryGetValue(key, out var recipe);
            return recipe;
        }
        
        public List<Recipe> GetRecipesForOutput(Element element)
        {
            return _recipes != null 
                ? _recipes.Where(r => r && r.output == element).ToList()
                : new List<Recipe>();
        }

    }
}
