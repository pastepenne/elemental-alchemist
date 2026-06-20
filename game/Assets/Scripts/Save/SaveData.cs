using System;
using System.Collections.Generic;
using ElementalAlchemist.Shared;

namespace ElementalAlchemist.Save
{
    public class SaveData
    {
        public string sceneName;
        public string spawnId;
        public ProgressionData progression = new();
        public ElementDefinition[] discoveredElements = Array.Empty<ElementDefinition>();
        public RecipeDefinition[] discoveredRecipes = Array.Empty<RecipeDefinition>();
        public Dictionary<string, int> inventory = new();
    }

    public class ProgressionData
    {
        public int stage;
        public bool hasBreath;
        public bool hasFlesh;
        public bool hasSoul;
        public bool hasWater;
        public bool hasAir;
        public bool hasEarth;
        public bool hasFire;
        public bool hasMenu;
        public bool isFreeplay;
    }
}
