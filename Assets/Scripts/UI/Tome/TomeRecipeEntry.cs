using ElementalAlchemist.Data;
using TMPro;
using UnityEngine;

namespace ElementalAlchemist.UI.Tome
{
    public class TomeRecipeEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _recipe;
        
        public void Setup(Recipe recipe)
        {
            _recipe.text = $"{recipe.inputA.displayName} + {recipe.inputB.displayName}";
        }
    }
}