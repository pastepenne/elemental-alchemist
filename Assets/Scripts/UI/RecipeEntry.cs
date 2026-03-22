using ElementalAlchemist.Data;
using TMPro;
using UnityEngine;

namespace ElementalAlchemist.UI
{
    public class RecipeEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _recipe;
        
        public void Setup(Recipe recipe)
        {
            _recipe.text = $"{recipe.inputA.displayName} + {recipe.inputB.displayName}";
        }
    }
}