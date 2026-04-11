using System.Collections.Generic;
using ElementalAlchemist.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElementalAlchemist.UI.Tome
{
    public class TomeDetails : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _tier;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private GameObject _recipeEntryPrefab;
        [SerializeField] private Transform _recipeContentGroup;
        [SerializeField] private RecipeCatalog _recipeCatalog;
        
        private readonly List<GameObject> _recipeEntries = new();
        
        public void Display(Element element)
        {
            _icon.sprite =  element.icon;
            _name.text = element.displayName;
            _tier.text = element.tier.ToString();
            _description.text = element.description;
            
            foreach (var slot in _recipeEntries)
            {
                Destroy(slot);
            }

            _recipeEntries.Clear();
            
            var recipes = _recipeCatalog.GetRecipesForOutput(element);
            foreach (var recipe in recipes)
            {
                var entryObject = Instantiate(_recipeEntryPrefab, _recipeContentGroup);
                entryObject.GetComponent<TomeRecipeEntry>().Setup(recipe);
                _recipeEntries.Add(entryObject);
            }
        }
    }
}