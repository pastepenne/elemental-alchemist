using System;
using System.Collections.Generic;
using ElementalAlchemist.Data;

namespace ElementalAlchemist.Player
{
    /// <summary>
    /// Tracks discovered elements and recipes.
    /// </summary>
    public class Discovery
    {
        private readonly HashSet<Element> _discoveredElements = new();
        private readonly HashSet<RecipeKey> _discoveredRecipes = new();

        public IReadOnlyCollection<Element> GetDiscoveredElements() => _discoveredElements;
        
        public bool IsElementDiscovered(Element element)
        {
            return _discoveredElements.Contains(element);
        }
        
        public bool IsRecipeDiscovered(Recipe recipe)
        {
            return _discoveredRecipes.Contains(new RecipeKey(recipe));
        }

        public void DiscoverElement(Element element)
        {
            _discoveredElements.Add(element);
        }
        
        public void DiscoverRecipe(Recipe recipe)
        {
            _discoveredRecipes.Add(new RecipeKey(recipe));
        }
    }
}
