using System;
using System.Collections.Generic;
using ElementalAlchemist.Element;
using ElementalAlchemist.Fusion;

namespace ElementalAlchemist.Player
{
    /// <summary>
    /// Tracks discovered elements and recipes.
    /// </summary>
    public class Discovery
    {
        private readonly HashSet<ElementData> _discoveredElements = new();
        private readonly HashSet<RecipeKey> _discoveredRecipes = new();

        public IReadOnlyCollection<ElementData> GetDiscoveredElements() => _discoveredElements;
        
        public bool IsElementDiscovered(ElementData element)
        {
            return _discoveredElements.Contains(element);
        }
        
        public bool IsRecipeDiscovered(RecipeData recipe)
        {
            return _discoveredRecipes.Contains(new RecipeKey(recipe));
        }

        public void DiscoverElement(ElementData element)
        {
            _discoveredElements.Add(element);
        }
        
        public void DiscoverRecipe(RecipeData recipe)
        {
            _discoveredRecipes.Add(new RecipeKey(recipe));
        }
    }
}
