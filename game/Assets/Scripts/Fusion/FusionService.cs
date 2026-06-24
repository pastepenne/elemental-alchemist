using System;
using System.Collections;
using System.Text;
using ElementalAlchemist.Element;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using ElementalAlchemist.Shared;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace ElementalAlchemist.Fusion
{
    public static class FusionService
    {
        private const int RequestTimeoutSeconds = 15;

        public static IEnumerator Fuse(
            RecipeCatalog catalog,
            ElementRegistry registry,
            string baseUrl,
            ElementData inputA,
            ElementData inputB,
            Action<FusionResult> onComplete)
        {
            var inventory = PlayerManager.Instance.Inventory;

            var recipe = catalog.FindRecipe(inputA, inputB);
            if (recipe)
            {
                var allowed = IsTierAllowed(recipe) && HasRequiredElements(inputA, inputB, inventory);
                onComplete?.Invoke(allowed
                    ? ApplyFusion(inputA, inputB, recipe.output)
                    : new FusionResult { Success = false });
                yield break;
            }

            if (!ProgressionManager.Instance.IsFreeplayActive || !HasRequiredElements(inputA, inputB, inventory))
            {
                onComplete?.Invoke(new FusionResult { Success = false });
                yield break;
            }

            var pair = new FusionPair(inputA.Id, inputB.Id);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pair, SerializerSettings));

            using var request = new UnityWebRequest($"{baseUrl}/fuse", UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.timeout = RequestTimeoutSeconds; // Bound the wait so a hung server can't trap the player in the menu.

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[FusionService] Fusion request failed ({request.responseCode}): {request.error}");
                onComplete?.Invoke(new FusionResult { Success = false });
                yield break;
            }

            if (request.responseCode == 204 || string.IsNullOrWhiteSpace(request.downloadHandler.text))
            {
                onComplete?.Invoke(new FusionResult { Success = false });
                yield break;
            }

            ElementDefinition definition;
            try
            {
                definition = JsonConvert.DeserializeObject<ElementDefinition>(request.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FusionService] Could not parse fusion response: {e.Message}");
                onComplete?.Invoke(new FusionResult { Success = false });
                yield break;
            }

            var output = registry ? registry.Resolve(definition) : null;
            if (!output)
            {
                onComplete?.Invoke(new FusionResult { Success = false });
                yield break;
            }

            onComplete?.Invoke(ApplyFusion(inputA, inputB, output));
        }

        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };

        private static FusionResult ApplyFusion(ElementData inputA, ElementData inputB, ElementData output)
        {
            var inventory = PlayerManager.Instance.Inventory;
            var discovery = PlayerManager.Instance.Discovery;

            ConsumeElements(inputA, inputB, inventory);
            inventory.AddElement(output);

            var recipe = RecipeData.CreateRuntime(inputA, inputB, output);
            var newDiscovery = !discovery.IsRecipeDiscovered(recipe);
            if (newDiscovery)
            {
                discovery.DiscoverRecipe(recipe);
            }

            discovery.DiscoverElement(output);

            return new FusionResult
            {
                Success = true,
                Output = output,
                NewDiscovery = newDiscovery
            };
        }

        private static bool IsTierAllowed(RecipeData recipe)
        {
            return recipe.output.Tier <= ProgressionManager.Instance.CurrentAllowedFusionTier;
        }

        private static bool HasRequiredElements(ElementData inputA, ElementData inputB, Inventory inventory)
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

        private static void ConsumeElements(ElementData inputA, ElementData inputB, Inventory inventory)
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
