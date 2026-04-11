using System;

namespace ElementalAlchemist.Data
{
    /// <summary>
    /// Order-independent key for a pair of element IDs, used for recipe lookup and discovery tracking.
    /// </summary>
    public readonly struct RecipeKey : IEquatable<RecipeKey>
    {
        private readonly string _idA;
        private readonly string _idB;

        public RecipeKey(string idA, string idB)
        {
            // Sort so that (A, B) and (B, A) produce the same key
            if (string.CompareOrdinal(idA, idB) <= 0)
            {
                _idA = idA;
                _idB = idB;
            }
            else
            {
                _idA = idB;
                _idB = idA;
            }
        }

        public RecipeKey(Recipe recipe) : this(recipe.inputA.id, recipe.inputB.id) { }

        public bool Equals(RecipeKey other) => _idA == other._idA && _idB == other._idB;
        public override bool Equals(object obj) => obj is RecipeKey other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_idA, _idB);
    }
}