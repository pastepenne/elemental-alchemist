using System.Collections.Generic;
using System.IO;
using ElementalAlchemist.Element;
using ElementalAlchemist.Fusion;
using ElementalAlchemist.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEngine;

namespace ElementalAlchemist.Editor
{
    public static class ContentExporter
    {
        private const string _outputDirRelativeToProject = "../../shared/Content";
        private const string _elementsFileName = "elements.json";
        private const string _recipesFileName = "recipes.json";

        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new StringEnumConverter() },
            Formatting = Formatting.Indented,
        };

        [MenuItem("Elemental Alchemist/Export Content")]
        public static void Export()
        {
            var outputDir = Path.GetFullPath(Path.Combine(Application.dataPath, _outputDirRelativeToProject));
            Directory.CreateDirectory(outputDir);

            var elementCount = ExportElements(Path.Combine(outputDir, _elementsFileName));
            var recipeCount = ExportRecipes(Path.Combine(outputDir, _recipesFileName));

            AssetDatabase.Refresh();

            var message = $"Exported {elementCount} elements and {recipeCount} recipes to:\n{outputDir}";
            Debug.Log($"[ContentExporter] {message}");
            EditorUtility.DisplayDialog("Export Server Content", message, "OK");
        }

        private static int ExportElements(string path)
        {
            var seen = new HashSet<string>();
            var elements = new List<ElementDefinition>();

            foreach (var element in LoadAll<ElementData>())
            {
                if (element.IsDynamic)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(element.Id))
                {
                    Debug.LogWarning($"[ContentExporter] Skipping element asset with empty id: {AssetDatabase.GetAssetPath(element)}", element);
                    continue;
                }

                if (!seen.Add(element.Id))
                {
                    Debug.LogWarning($"[ContentExporter] Duplicate element id '{element.Id}' skipped: {AssetDatabase.GetAssetPath(element)}", element);
                    continue;
                }

                elements.Add(new ElementDefinition
                {
                    Id = element.Id,
                    DisplayName = element.DisplayName,
                    Description = element.Description,
                    Tier = element.Tier,
                    Tags = element.Tags ?? System.Array.Empty<string>(),
                });
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(elements, _jsonSettings));
            return elements.Count;
        }

        private static int ExportRecipes(string path)
        {
            var recipes = new List<RecipeDefinition>();

            foreach (var recipe in LoadAll<RecipeData>())
            {
                if (!recipe.inputA || !recipe.inputB || !recipe.output)
                {
                    Debug.LogWarning($"[ContentExporter] Skipping recipe with a missing slot: {AssetDatabase.GetAssetPath(recipe)}", recipe);
                    continue;
                }

                recipes.Add(new RecipeDefinition
                {
                    ElementA = recipe.inputA.Id,
                    ElementB = recipe.inputB.Id,
                    Result = recipe.output.Id,
                });
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(recipes, _jsonSettings));
            return recipes.Count;
        }

        private static IEnumerable<T> LoadAll<T>() where T : Object
        {
            foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(T).Name}"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset)
                {
                    yield return asset;
                }
            }
        }
    }
}
