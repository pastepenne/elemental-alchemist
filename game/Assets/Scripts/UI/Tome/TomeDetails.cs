using System.Collections.Generic;
using ElementalAlchemist.Element;
using ElementalAlchemist.Fusion;
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
        
        public void Display(ElementData element)
        {
            _icon.sprite =  element.Icon;
            _name.text = element.DisplayName;
            _tier.text = element.Tier.ToString();
            _description.text = element.Description;
            
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