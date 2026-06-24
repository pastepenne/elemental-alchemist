using System.Collections.Generic;
using ElementalAlchemist.Element;
using ElementalAlchemist.Fusion;
using ElementalAlchemist.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Tome
{
    public class TomeDetails : MonoBehaviour
    {
        [SerializeField] private Image[] _icons;
        [SerializeField] private TagSpriteLibrary _tagSprites;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _tier;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private GameObject _recipeEntryPrefab;
        [SerializeField] private Transform _recipeContentGroup;
        [SerializeField] private RecipeCatalog _recipeCatalog;
        
        private readonly List<GameObject> _recipeEntries = new();
        
        public void Display(ElementData element)
        {
            ElementIcons.Apply(_icons, element, _tagSprites);
            _name.text = element.DisplayName;
            _tier.text = element.Tier.ToString();
            _description.text = element.Description;
            
            foreach (var slot in _recipeEntries)
            {
                Destroy(slot);
            }

            _recipeEntries.Clear();
            
            foreach (var recipe in CollectRecipes(element))
            {
                var entryObject = Instantiate(_recipeEntryPrefab, _recipeContentGroup);
                entryObject.GetComponent<TomeRecipeEntry>().Setup(recipe);
                _recipeEntries.Add(entryObject);
            }
        }

        /// <summary>Static catalog recipes plus the player's discovered ones (incl. dynamic fusions), deduped by input pair.</summary>
        private List<RecipeData> CollectRecipes(ElementData element)
        {
            var seen = new HashSet<RecipeKey>();
            var recipes = new List<RecipeData>();

            foreach (var recipe in _recipeCatalog.GetRecipesForOutput(element))
            {
                if (seen.Add(new RecipeKey(recipe)))
                {
                    recipes.Add(recipe);
                }
            }

            foreach (var recipe in PlayerManager.Instance.Discovery.GetDiscoveredRecipes())
            {
                if (recipe.output == element && seen.Add(new RecipeKey(recipe)))
                {
                    recipes.Add(recipe);
                }
            }

            return recipes;
        }
    }
}