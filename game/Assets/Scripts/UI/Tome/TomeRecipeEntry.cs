using ElementalAlchemist.Fusion;
using TMPro;
using UnityEngine;

namespace ElementalAlchemist.UI.Tome
{
    public class TomeRecipeEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _recipe;
        
        public void Setup(RecipeData recipe)
        {
            _recipe.text = $"{recipe.inputA.DisplayName} + {recipe.inputB.DisplayName}";
        }
    }
}