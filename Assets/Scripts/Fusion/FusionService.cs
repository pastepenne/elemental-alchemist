using ElementalAlchemist.Data;
using ElementalAlchemist.Player;

namespace ElementalAlchemist.Fusion
{
    public static class FusionService
    {
        public static FusionResult TryFuse(RecipeCatalog catalog, Element inputA, Element inputB)
        {
            var inventory = PlayerManager.Instance.Inventory;
            var discovery = PlayerManager.Instance.Discovery;
            
            var recipe = catalog.FindRecipe(inputA, inputB);
            if (!recipe || !HasRequiredElements(inputA, inputB, inventory))
            {
                return new FusionResult { Success = false };
            }

            ConsumeElements(inputA, inputB, inventory);

            inventory.AddElement(recipe.output);

            var newDiscovery = !discovery.IsRecipeDiscovered(recipe);
            if (newDiscovery)
            {
                discovery.DiscoverRecipe(recipe);
            }

            return new FusionResult
            {
                Success = true,
                Output = recipe.output,
                NewDiscovery = newDiscovery
            };
        }

        private static bool HasRequiredElements(Element inputA, Element inputB, Inventory inventory)
        {
            if (inputA.IsCore && inputB.IsCore)
            {
                return true;
            }

            if (inputA == inputB)
            {
                return inventory.HasElement(inputA, 2);
            }

            if (!inputA.IsCore && !inventory.HasElement(inputA))
            {
                return false;
            }

            if (!inputB.IsCore && !inventory.HasElement(inputB))
            {
                return false;
            }

            return true;
        }

        private static void ConsumeElements(Element inputA, Element inputB, Inventory inventory)
        {
            if (inputA.IsCore && inputB.IsCore)
            {
                return;
            }

            if (inputA == inputB)
            {
                inventory.RemoveElement(inputA, 2);
                return;
            }

            if (!inputA.IsCore)
            {
                inventory.RemoveElement(inputA);
            }

            if (!inputB.IsCore)
            {
                inventory.RemoveElement(inputB);
            }
        }
    }
}
