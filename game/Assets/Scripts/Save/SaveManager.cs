using System.Collections.Generic;
using System.IO;
using System.Linq;
using ElementalAlchemist.Element;
using ElementalAlchemist.Fusion;
using ElementalAlchemist.Player;
using ElementalAlchemist.Progression;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ElementalAlchemist.Save
{
    public class SaveManager : MonoBehaviour
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        public static SaveManager Instance { get; private set; }

        [SerializeField] private ElementRegistry _registry;
        [SerializeField] private string _menuSceneName = "MainMenu";

        private SaveData _pendingLoad;
        private string _currentSpawnId;
        private bool _restoring;

        public static bool HasSave() => File.Exists(SavePath);

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            StoryDirector.SequenceCompleted += OnCheckpoint;
            ProgressionManager.CoreUnlocked += OnCoreUnlocked;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            StoryDirector.SequenceCompleted -= OnCheckpoint;
            ProgressionManager.CoreUnlocked -= OnCoreUnlocked;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }

        public void Save()
        {
            if (_restoring)
            {
                return;
            }

            // The menu is never a resume point. This is the hard guard: SaveManager persists into the menu, so its
            // scene-load autosave would otherwise overwrite a good save with sceneName=menu (the gameplay managers
            // are only Destroy()'d at end of frame, so the null check below still passes during that load).
            if (SceneManager.GetActiveScene().name == _menuSceneName)
            {
                return;
            }

            var progression = ProgressionManager.Instance;
            var player = PlayerManager.Instance;
            if (!progression || !player)
            {
                return;
            }

            var data = new SaveData
            {
                sceneName = SceneManager.GetActiveScene().name,
                spawnId = _currentSpawnId,
                progression = progression.CaptureState(),
                discoveredElements = player.Discovery.GetDiscoveredElements().Select(e => e.ToDefinition()).ToArray(),
                discoveredRecipes = player.Discovery.GetDiscoveredRecipes().Select(r => r.ToDefinition()).ToArray(),
                inventory = player.Inventory.GetStacks()
                    .Where(stack => stack.Element)
                    .ToDictionary(stack => stack.Element.Id, stack => stack.Quantity)
            };

            File.WriteAllText(SavePath, JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        public void Load()
        {
            if (!HasSave())
            {
                return;
            }

            var data = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(SavePath));
            if (data == null || string.IsNullOrEmpty(data.sceneName))
            {
                return;
            }

            _pendingLoad = data;
            SceneManager.LoadScene(data.sceneName);
        }

        public void DeleteSave()
        {
            if (HasSave())
            {
                File.Delete(SavePath);
            }
        }

        private void ApplyState(SaveData data)
        {
            _restoring = true;

            if (ProgressionManager.Instance)
            {
                ProgressionManager.Instance.RestoreState(data.progression);
            }

            if (PlayerManager.Instance && _registry)
            {
                var lookup = new Dictionary<string, ElementData>();
                
                foreach (var definition in data.discoveredElements)
                {
                    var element = _registry.Resolve(definition);
                    if (element)
                    {
                        PlayerManager.Instance.Discovery.DiscoverElement(element);
                        lookup[element.Id] = element;
                    }
                }
                
                foreach (var recipe in data.discoveredRecipes)
                {
                    if (lookup.TryGetValue(recipe.ElementA, out var a) &&
                        lookup.TryGetValue(recipe.ElementB, out var b) &&
                        lookup.TryGetValue(recipe.Result, out var output))
                    {
                        PlayerManager.Instance.Discovery.DiscoverRecipe(RecipeData.CreateRuntime(a, b, output));
                    }
                }

                foreach (var entry in data.inventory)
                {
                    if (lookup.TryGetValue(entry.Key, out var element))
                    {
                        PlayerManager.Instance.Inventory.AddElement(element, entry.Value);
                    }
                }
            }

            PlayerSpawner.PendingSpawnId = data.spawnId;
            _currentSpawnId = data.spawnId;

            _restoring = false;
        }

        private void OnCheckpoint(string sequenceId) => Save();
        private void OnCoreUnlocked(ElementData element) => Save();
        private void OnApplicationQuit() => Save();
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _currentSpawnId = PlayerSpawner.PendingSpawnId;

            if (_pendingLoad != null)
            {
                var data = _pendingLoad;
                _pendingLoad = null;
                ApplyState(data);
                return;
            }

            Save();
        }
        
        [ContextMenu("Debug/Save Now")]
        private void DebugSave()
        {
            Save();
            Debug.Log($"[SaveManager] Saved to {SavePath}");
        }

        [ContextMenu("Debug/Load")]
        private void DebugLoad() => Load();

        [ContextMenu("Debug/Delete Save")]
        private void DebugDelete()
        {
            DeleteSave();
            Debug.Log("[SaveManager] Save deleted");
        }
    }
}
