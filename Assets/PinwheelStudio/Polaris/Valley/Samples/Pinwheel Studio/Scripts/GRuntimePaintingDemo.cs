#if GRIFFIN
using System.Collections.Generic;
using Pinwheel.Griffin.PaintTool;
using UnityEngine;
using UnityEngine.Rendering;

namespace Pinwheel.Griffin
{
    public class GRuntimePaintingDemo : MonoBehaviour
    {
        private enum GPaintCategory
        {
            Texture,
            Foliage,
            Object
        }

        [Header("Bootstrap")]
        [SerializeField]
        private bool bootstrapOnPlay = true;
        [SerializeField]
        private bool createCameraIfMissing = true;
        [SerializeField]
        private bool createDirectionalLightIfMissing = true;
        [SerializeField]
        private int terrainGroupId;
        [SerializeField]
        private Vector3 terrainSize = new Vector3(128f, 32f, 128f);
        [SerializeField]
        private GLightingModel lightingModel = GLightingModel.PBR;
        [SerializeField]
        private GTexturingModel shadingMode = GTexturingModel.Splat;

        [Header("Optional Demo Content")]
        [SerializeField]
        private Texture2D[] splatTextures;
        [SerializeField]
        private Texture2D[] brushTextures;
        [SerializeField]
        private GameObject[] treePrefabs;
        [SerializeField]
        private GameObject[] grassPrefabs;
        [SerializeField]
        private GameObject[] objectPrefabs;

        [Header("UI")]
        [SerializeField]
        private bool showUi = true;
        [SerializeField]
        private Rect windowRect = new Rect(12, 12, 360, 620);

        [Header("Camera")]
        [SerializeField]
        private float cameraMoveSpeed = 20f;
        [SerializeField]
        private float cameraLookSpeed = 3f;
        [SerializeField]
        private float cameraBoostMultiplier = 3f;

        private GStylizedTerrain terrain;
        private GTerrainTexturePainter texturePainter;
        private GFoliagePainter foliagePainter;
        private GObjectPainter objectPainter;
        private GSplatPrototypeGroup runtimeSplatGroup;
        private GTreePrototypeGroup runtimeTreeGroup;
        private GGrassPrototypeGroup runtimeGrassGroup;
        private GameObject templateRoot;
        private GameObject cursorSphere;
        private Camera runtimeCamera;
        private Light runtimeLight;

        private readonly List<Object> runtimeAssets = new List<Object>();
        private readonly List<Texture2D> generatedBrushTextures = new List<Texture2D>();
        private readonly List<Texture2D> generatedSplatTextures = new List<Texture2D>();
        private readonly List<GameObject> generatedTreePrefabs = new List<GameObject>();
        private readonly List<GameObject> generatedGrassPrefabs = new List<GameObject>();
        private readonly List<GameObject> generatedObjectPrefabs = new List<GameObject>();

        private GPaintCategory paintCategory = GPaintCategory.Texture;
        private Vector2 scrollPosition;
        private bool initialized;
        private bool wasHoldingPaint;
        private bool hasLastPaintHit;
        private Vector3 lastPaintHitPoint;
        private bool hasCursorHit;
        private RaycastHit cursorHit;
        private float cameraYaw;
        private float cameraPitch;
        private GTexturingModel appliedShadingMode = (GTexturingModel)(-1);

        private static readonly string[] PAINT_CATEGORY_LABELS = { "Texture", "Foliage", "Object" };
        private static readonly string[] SHADING_LABELS = { "Gradient", "Color Map", "Splat", "Vertex" };

        private void Start()
        {
            if (Application.isPlaying && bootstrapOnPlay)
            {
                EnsureInitialized();
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
                return;

            if (!bootstrapOnPlay)
                return;

            EnsureInitialized();
            UpdateCursorHit();
            UpdatePainting();
            UpdateCursorSphere();
            UpdateCameraControls();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying || !showUi)
                return;

            EnsureInitialized();
            windowRect = GUILayout.Window(GetInstanceID(), windowRect, DrawWindow, "Polaris Runtime Painting Demo");
        }

        private void OnDestroy()
        {
            CleanupRuntimeAssets();
        }

        private void EnsureInitialized()
        {
            if (initialized)
                return;

            EnsureTerrain();
            EnsurePainters();
            EnsureBrushTextures();
            EnsurePrototypeData();
            ApplyBrushTextures();
            ApplyPainterDefaults();
            ApplyShadingMode(shadingMode, true);
            EnsureRuntimeCamera();
            EnsureRuntimeLight();
            EnsureCursorSphere();
            initialized = true;
        }

        private void EnsureTerrain()
        {
            terrain = GetComponentInChildren<GStylizedTerrain>();
            if (terrain != null && terrain.TerrainData != null)
            {
                terrain.GroupId = terrainGroupId;
                return;
            }

            GTerrainData terrainData = ScriptableObject.CreateInstance<GTerrainData>();
            RegisterRuntimeAsset(terrainData);
            terrainData.Reset();
            terrainData.Geometry.Reset();
            terrainData.Shading.Reset();
            terrainData.Rendering.Reset();
            terrainData.Foliage.Reset();
            terrainData.Mask.Reset();
            RegisterRuntimeAsset(terrainData.Geometry);
            RegisterRuntimeAsset(terrainData.Shading);
            RegisterRuntimeAsset(terrainData.Rendering);
            RegisterRuntimeAsset(terrainData.Foliage);
            RegisterRuntimeAsset(terrainData.Mask);
            terrainData.Geometry.Width = terrainSize.x;
            terrainData.Geometry.Height = terrainSize.y;
            terrainData.Geometry.Length = terrainSize.z;

            GameObject terrainGo = new GameObject("Runtime Terrain");
            terrainGo.transform.SetParent(transform, false);
            terrainGo.transform.localPosition = new Vector3(-terrainSize.x * 0.5f, 0f, -terrainSize.z * 0.5f);
            RegisterRuntimeAsset(terrainGo);

            terrain = terrainGo.AddComponent<GStylizedTerrain>();
            terrain.GroupId = terrainGroupId;
            terrain.TerrainData = terrainData;
        }

        private void EnsurePainters()
        {
            texturePainter = GetComponent<GTerrainTexturePainter>();
            if (texturePainter == null)
            {
                texturePainter = gameObject.AddComponent<GTerrainTexturePainter>();
            }

            foliagePainter = GetComponent<GFoliagePainter>();
            if (foliagePainter == null)
            {
                foliagePainter = gameObject.AddComponent<GFoliagePainter>();
            }

            objectPainter = GetComponent<GObjectPainter>();
            if (objectPainter == null)
            {
                objectPainter = gameObject.AddComponent<GObjectPainter>();
            }
        }

        private void EnsureBrushTextures()
        {
            generatedBrushTextures.Clear();

            if (brushTextures != null)
            {
                for (int i = 0; i < brushTextures.Length; ++i)
                {
                    if (brushTextures[i] != null)
                    {
                        generatedBrushTextures.Add(brushTextures[i]);
                    }
                }
            }

            if (generatedBrushTextures.Count > 0)
                return;

            generatedBrushTextures.Add(CreateBrushTexture("Soft Circle", 64, 2.5f, false));
            generatedBrushTextures.Add(CreateBrushTexture("Hard Circle", 64, 8f, false));
            generatedBrushTextures.Add(CreateBrushTexture("Square", 64, 1f, true));
        }

        private void EnsurePrototypeData()
        {
            EnsureTemplateRoot();
            EnsureSplatGroup();
            EnsureTreeGroup();
            EnsureGrassGroup();
            EnsureObjectPrototypeList();
        }

        private void EnsureTemplateRoot()
        {
            if (templateRoot != null)
                return;

            templateRoot = new GameObject("Runtime Demo Templates");
            templateRoot.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
            templateRoot.transform.SetParent(transform, false);
            templateRoot.transform.localPosition = new Vector3(0f, -1000f, 0f);
            RegisterRuntimeAsset(templateRoot);
        }

        private void EnsureSplatGroup()
        {
            if (runtimeSplatGroup == null)
            {
                runtimeSplatGroup = ScriptableObject.CreateInstance<GSplatPrototypeGroup>();
                runtimeSplatGroup.name = "Runtime Splat Group";
                RegisterRuntimeAsset(runtimeSplatGroup);
            }

            runtimeSplatGroup.Prototypes.Clear();
            generatedSplatTextures.Clear();

            List<Texture2D> textures = new List<Texture2D>();
            if (splatTextures != null)
            {
                for (int i = 0; i < splatTextures.Length; ++i)
                {
                    if (splatTextures[i] != null)
                    {
                        textures.Add(splatTextures[i]);
                    }
                }
            }

            if (textures.Count == 0)
            {
                textures.Add(CreateCheckerTexture("Grass Splat", new Color(0.26f, 0.46f, 0.18f), new Color(0.19f, 0.34f, 0.13f)));
                textures.Add(CreateCheckerTexture("Dirt Splat", new Color(0.43f, 0.31f, 0.18f), new Color(0.33f, 0.22f, 0.12f)));
                textures.Add(CreateCheckerTexture("Sand Splat", new Color(0.71f, 0.64f, 0.42f), new Color(0.58f, 0.51f, 0.34f)));
                textures.Add(CreateCheckerTexture("Rock Splat", new Color(0.48f, 0.48f, 0.50f), new Color(0.36f, 0.36f, 0.38f)));
            }

            for (int i = 0; i < textures.Count; ++i)
            {
                GSplatPrototype proto = new GSplatPrototype();
                proto.Texture = textures[i];
                proto.TileSize = new Vector2(12f, 12f);
                proto.Metallic = 0f;
                proto.Smoothness = 0.1f;
                runtimeSplatGroup.Prototypes.Add(proto);
            }

            terrain.TerrainData.Shading.Splats = runtimeSplatGroup;
        }

        private void EnsureTreeGroup()
        {
            if (runtimeTreeGroup == null)
            {
                runtimeTreeGroup = ScriptableObject.CreateInstance<GTreePrototypeGroup>();
                runtimeTreeGroup.name = "Runtime Tree Group";
                RegisterRuntimeAsset(runtimeTreeGroup);
            }

            runtimeTreeGroup.Prototypes.Clear();
            generatedTreePrefabs.Clear();

            List<GameObject> prefabs = new List<GameObject>();
            if (treePrefabs != null)
            {
                for (int i = 0; i < treePrefabs.Length; ++i)
                {
                    if (treePrefabs[i] != null)
                    {
                        prefabs.Add(treePrefabs[i]);
                    }
                }
            }

            if (prefabs.Count == 0)
            {
                prefabs.Add(CreateTreeTemplate("Demo Tree A", new Color(0.30f, 0.20f, 0.12f), new Color(0.20f, 0.48f, 0.18f), 1.6f));
                prefabs.Add(CreateTreeTemplate("Demo Tree B", new Color(0.26f, 0.18f, 0.10f), new Color(0.32f, 0.58f, 0.20f), 2.1f));
            }

            for (int i = 0; i < prefabs.Count; ++i)
            {
                runtimeTreeGroup.Prototypes.Add(GTreePrototype.Create(prefabs[i]));
            }

            runtimeTreeGroup.NotifyChanged();
            terrain.TerrainData.Foliage.Trees = runtimeTreeGroup;
        }

        private void EnsureGrassGroup()
        {
            if (runtimeGrassGroup == null)
            {
                runtimeGrassGroup = ScriptableObject.CreateInstance<GGrassPrototypeGroup>();
                runtimeGrassGroup.name = "Runtime Grass Group";
                RegisterRuntimeAsset(runtimeGrassGroup);
            }

            runtimeGrassGroup.Prototypes.Clear();
            generatedGrassPrefabs.Clear();

            List<GameObject> prefabs = new List<GameObject>();
            if (grassPrefabs != null)
            {
                for (int i = 0; i < grassPrefabs.Length; ++i)
                {
                    if (grassPrefabs[i] != null)
                    {
                        prefabs.Add(grassPrefabs[i]);
                    }
                }
            }

            if (prefabs.Count == 0)
            {
                prefabs.Add(CreateGrassTemplate("Demo Grass A", new Color(0.26f, 0.72f, 0.24f), new Vector3(0.55f, 1.1f, 1f)));
                prefabs.Add(CreateGrassTemplate("Demo Grass B", new Color(0.42f, 0.68f, 0.18f), new Vector3(0.8f, 1.5f, 1f)));
            }

            for (int i = 0; i < prefabs.Count; ++i)
            {
                runtimeGrassGroup.Prototypes.Add(GGrassPrototype.Create(prefabs[i]));
            }

            runtimeGrassGroup.NotifyChanged();
            terrain.TerrainData.Foliage.Grasses = runtimeGrassGroup;
        }

        private void EnsureObjectPrototypeList()
        {
            generatedObjectPrefabs.Clear();

            if (objectPrefabs != null)
            {
                for (int i = 0; i < objectPrefabs.Length; ++i)
                {
                    if (objectPrefabs[i] != null)
                    {
                        generatedObjectPrefabs.Add(objectPrefabs[i]);
                    }
                }
            }

            if (generatedObjectPrefabs.Count == 0)
            {
                generatedObjectPrefabs.Add(CreateRockTemplate("Demo Rock", PrimitiveType.Sphere, new Color(0.45f, 0.45f, 0.47f), new Vector3(1.3f, 0.9f, 1.1f)));
                generatedObjectPrefabs.Add(CreateRockTemplate("Demo Crate", PrimitiveType.Cube, new Color(0.56f, 0.40f, 0.22f), new Vector3(1f, 1f, 1f)));
            }
        }

        private void ApplyBrushTextures()
        {
            texturePainter.BrushMasks = new List<Texture2D>(generatedBrushTextures);
            foliagePainter.BrushMasks = new List<Texture2D>(generatedBrushTextures);
            objectPainter.BrushMasks = new List<Texture2D>(generatedBrushTextures);

            texturePainter.SelectedBrushMaskIndex = Mathf.Clamp(texturePainter.SelectedBrushMaskIndex, 0, generatedBrushTextures.Count - 1);
            foliagePainter.SelectedBrushMaskIndex = Mathf.Clamp(foliagePainter.SelectedBrushMaskIndex, 0, generatedBrushTextures.Count - 1);
            objectPainter.SelectedBrushMaskIndex = Mathf.Clamp(objectPainter.SelectedBrushMaskIndex, 0, generatedBrushTextures.Count - 1);
        }

        private void ApplyPainterDefaults()
        {
            terrain.GroupId = terrainGroupId;
            texturePainter.GroupId = terrainGroupId;
            foliagePainter.GroupId = terrainGroupId;
            objectPainter.GroupId = terrainGroupId;

            texturePainter.Mode = GTexturePaintingMode.Splat;
            texturePainter.BrushRadius = 12f;
            texturePainter.BrushOpacity = 0.8f;
            texturePainter.BrushTargetStrength = 1f;
            texturePainter.BrushColor = Color.white;
            texturePainter.EnableTerrainMask = false;
            if (texturePainter.SelectedSplatIndices.Count == 0)
            {
                texturePainter.SelectedSplatIndices = new List<int> { 0 };
            }

            foliagePainter.Mode = GFoliagePaintingMode.PaintTree;
            foliagePainter.BrushRadius = 12f;
            foliagePainter.BrushDensity = 4;
            foliagePainter.EraseRatio = 1f;
            foliagePainter.ScaleStrength = 1f;
            foliagePainter.EnableTerrainMask = false;
            if (foliagePainter.SelectedTreeIndices.Count == 0)
            {
                foliagePainter.SelectedTreeIndices = new List<int> { 0 };
            }
            if (foliagePainter.SelectedGrassIndices.Count == 0)
            {
                foliagePainter.SelectedGrassIndices = new List<int> { 0 };
            }

            objectPainter.Mode = GObjectPaintingMode.Spawn;
            objectPainter.BrushRadius = 10f;
            objectPainter.BrushDensity = 2;
            objectPainter.EraseRatio = 1f;
            objectPainter.ScaleStrength = 1f;
            objectPainter.EnableTerrainMask = false;
            objectPainter.Prototypes = new List<GameObject>(generatedObjectPrefabs);
            if (objectPainter.SelectedPrototypeIndices.Count == 0)
            {
                objectPainter.SelectedPrototypeIndices = new List<int> { 0 };
            }
        }

        private void ApplyShadingMode(GTexturingModel mode, bool force = false)
        {
            if (!force && appliedShadingMode == mode)
                return;

            Material mat = GRuntimeSettings.Instance.terrainRendering.GetClonedMaterial(
                GCommon.CurrentRenderPipeline,
                lightingModel,
                mode,
                GSplatsModel.Splats4);

            if (mat != null)
            {
                RegisterRuntimeAsset(mat);
                terrain.TerrainData.Shading.CustomMaterial = mat;
            }

            terrain.TerrainData.Shading.Splats = runtimeSplatGroup;
            terrain.TerrainData.Geometry.AlbedoToVertexColorMode =
                mode == GTexturingModel.VertexColor ?
                GAlbedoToVertexColorMode.Sharp :
                GAlbedoToVertexColorMode.None;

            if (mode == GTexturingModel.GradientLookup)
            {
                terrain.TerrainData.Shading.UpdateLookupTextures();
            }

            terrain.TerrainData.Geometry.SetRegionDirty(new Rect(0f, 0f, 1f, 1f));
            terrain.TerrainData.SetDirty(GTerrainData.DirtyFlags.Shading | GTerrainData.DirtyFlags.Geometry);
            terrain.TerrainData.Shading.UpdateMaterials();
            shadingMode = mode;
            appliedShadingMode = mode;
        }

        private void EnsureRuntimeCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                runtimeCamera = mainCamera;
                Vector3 euler = runtimeCamera.transform.eulerAngles;
                cameraYaw = euler.y;
                cameraPitch = euler.x > 180f ? euler.x - 360f : euler.x;
                return;
            }

            if (!createCameraIfMissing)
                return;

            GameObject cameraGo = new GameObject("Runtime Painting Camera");
            cameraGo.tag = "MainCamera";
            cameraGo.transform.position = terrain.transform.position + new Vector3(0f, terrainSize.y * 1.3f, -terrainSize.z * 0.9f);
            cameraGo.transform.LookAt(GetTerrainCenterWorld());
            runtimeCamera = cameraGo.AddComponent<Camera>();
            cameraGo.AddComponent<AudioListener>();
            RegisterRuntimeAsset(cameraGo);

            Vector3 eulerAngles = cameraGo.transform.eulerAngles;
            cameraYaw = eulerAngles.y;
            cameraPitch = eulerAngles.x > 180f ? eulerAngles.x - 360f : eulerAngles.x;
        }

        private void EnsureRuntimeLight()
        {
            if (FindObjectOfType<Light>() != null || !createDirectionalLightIfMissing)
                return;

            GameObject lightGo = new GameObject("Runtime Painting Sun");
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            runtimeLight = lightGo.AddComponent<Light>();
            runtimeLight.type = LightType.Directional;
            runtimeLight.intensity = 1f;
            RegisterRuntimeAsset(lightGo);
        }

        private void EnsureCursorSphere()
        {
            if (cursorSphere != null)
                return;

            cursorSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cursorSphere.name = "Paint Cursor";
            cursorSphere.transform.SetParent(transform, false);
            cursorSphere.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
            RegisterRuntimeAsset(cursorSphere);

            Collider col = cursorSphere.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }

            MeshRenderer renderer = cursorSphere.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material mat = CreateCursorMaterial();
                renderer.sharedMaterial = mat;
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }

        private void UpdateCursorHit()
        {
            hasCursorHit = false;
            if (runtimeCamera == null)
                return;

            Ray ray = runtimeCamera.ScreenPointToRay(Input.mousePosition);
            hasCursorHit = GStylizedTerrain.Raycast(ray, out cursorHit, 5000f, terrainGroupId);
        }

        private void UpdatePainting()
        {
            bool pointerInsideUi = windowRect.Contains(GetGuiMousePosition());
            bool mouseHeld = Input.GetMouseButton(0);
            bool mouseDown = Input.GetMouseButtonDown(0);
            bool mouseUp = wasHoldingPaint && !mouseHeld;

            if (mouseDown && !pointerInsideUi && hasCursorHit)
            {
                DispatchPaint(GPainterMouseEventType.Down, cursorHit.point);
                lastPaintHitPoint = cursorHit.point;
                hasLastPaintHit = true;
            }
            else if (mouseHeld && !pointerInsideUi && hasCursorHit)
            {
                DispatchPaint(GPainterMouseEventType.Drag, cursorHit.point);
                lastPaintHitPoint = cursorHit.point;
                hasLastPaintHit = true;
            }
            else if (mouseUp && hasLastPaintHit)
            {
                DispatchPaint(GPainterMouseEventType.Up, lastPaintHitPoint);
            }

            wasHoldingPaint = mouseHeld;
            if (mouseUp)
            {
                hasLastPaintHit = false;
            }
        }

        private void DispatchPaint(GPainterMouseEventType mouseEventType, Vector3 hitPoint)
        {
            GPainterActionType actionType =
                IsShiftPressed() ? GPainterActionType.Alternative :
                IsCtrlPressed() ? GPainterActionType.Negative :
                GPainterActionType.Normal;

            if (paintCategory == GPaintCategory.Texture)
            {
                GTexturePainterArgs args = new GTexturePainterArgs
                {
                    HitPoint = hitPoint,
                    MouseEventType = mouseEventType,
                    ActionType = actionType
                };
                texturePainter.Paint(args);
            }
            else if (paintCategory == GPaintCategory.Foliage)
            {
                GFoliagePainterArgs args = new GFoliagePainterArgs
                {
                    HitPoint = hitPoint,
                    MouseEventType = mouseEventType,
                    ActionType = actionType
                };
                foliagePainter.Paint(args);
            }
            else
            {
                GObjectPainterArgs args = new GObjectPainterArgs
                {
                    HitPoint = hitPoint,
                    MouseEventType = mouseEventType,
                    ActionType = actionType
                };
                objectPainter.Paint(args);
            }
        }

        private void UpdateCursorSphere()
        {
            if (cursorSphere == null)
                return;

            if (!hasCursorHit)
            {
                cursorSphere.SetActive(false);
                return;
            }

            cursorSphere.SetActive(true);
            cursorSphere.transform.position = cursorHit.point;
            float radius = GetActiveBrushRadius();
            cursorSphere.transform.localScale = Vector3.one * radius * 2f;
        }

        private void UpdateCameraControls()
        {
            if (runtimeCamera == null || runtimeCamera != Camera.main)
                return;

            float speed = cameraMoveSpeed * (Time.unscaledDeltaTime > 0f ? Time.unscaledDeltaTime : 0.016f);
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                speed *= cameraBoostMultiplier;
            }

            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                move += runtimeCamera.transform.forward;
            if (Input.GetKey(KeyCode.S))
                move -= runtimeCamera.transform.forward;
            if (Input.GetKey(KeyCode.D))
                move += runtimeCamera.transform.right;
            if (Input.GetKey(KeyCode.A))
                move -= runtimeCamera.transform.right;
            if (Input.GetKey(KeyCode.E))
                move += Vector3.up;
            if (Input.GetKey(KeyCode.Q))
                move -= Vector3.up;

            if (move.sqrMagnitude > 0f)
            {
                runtimeCamera.transform.position += move.normalized * speed;
            }

            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0f)
            {
                runtimeCamera.transform.position += runtimeCamera.transform.forward * scroll * speed * 8f;
            }

            if (Input.GetMouseButton(1))
            {
                cameraYaw += Input.GetAxis("Mouse X") * cameraLookSpeed;
                cameraPitch -= Input.GetAxis("Mouse Y") * cameraLookSpeed;
                cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
                runtimeCamera.transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
            }
        }

        private void DrawWindow(int id)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("Left Mouse: paint");
            GUILayout.Label("Ctrl + Left Mouse: erase");
            GUILayout.Label("Shift + Left Mouse: alternative action");
            GUILayout.Label("Right Mouse + WASD/QE: camera");
            GUILayout.Space(6f);

            DrawSetupGui();
            DrawShadingGui();
            DrawCategoryGui();
            DrawBrushMaskGui();
            DrawToolGui();

            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
        }

        private void DrawSetupGui()
        {
            GUILayout.Label("Scene", GUI.skin.box);
            GUILayout.Label(string.Format("Terrain: {0}", terrain != null ? terrain.name : "None"));
            GUILayout.Label(string.Format("Group Id: {0}", terrainGroupId));

            if (GUILayout.Button("Rebuild Runtime Demo"))
            {
                CleanupRuntimeAssets();
                initialized = false;
                EnsureInitialized();
            }
        }

        private void DrawShadingGui()
        {
            GUILayout.Space(4f);
            GUILayout.Label("Terrain Shading", GUI.skin.box);
            int nextIndex = GUILayout.Toolbar((int)shadingMode, SHADING_LABELS);
            GTexturingModel nextMode = (GTexturingModel)nextIndex;
            if (nextMode != shadingMode)
            {
                ApplyShadingMode(nextMode);
            }
        }

        private void DrawCategoryGui()
        {
            GUILayout.Space(4f);
            GUILayout.Label("Painter", GUI.skin.box);
            paintCategory = (GPaintCategory)GUILayout.Toolbar((int)paintCategory, PAINT_CATEGORY_LABELS);
        }

        private void DrawBrushMaskGui()
        {
            GUILayout.Space(4f);
            GUILayout.Label("Brush Texture", GUI.skin.box);

            int selectedIndex = GetActiveBrushMaskIndex();
            GUILayout.BeginHorizontal();
            for (int i = 0; i < generatedBrushTextures.Count; ++i)
            {
                GUI.color = i == selectedIndex ? new Color(0.7f, 1f, 0.7f, 1f) : Color.white;
                if (GUILayout.Button(generatedBrushTextures[i], GUILayout.Width(64f), GUILayout.Height(64f)))
                {
                    SetActiveBrushMaskIndex(i);
                }
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }

        private void DrawToolGui()
        {
            GUILayout.Space(4f);
            if (paintCategory == GPaintCategory.Texture)
            {
                DrawTextureToolGui();
            }
            else if (paintCategory == GPaintCategory.Foliage)
            {
                DrawFoliageToolGui();
            }
            else
            {
                DrawObjectToolGui();
            }
        }

        private void DrawTextureToolGui()
        {
            GUILayout.Label("Texture Tool", GUI.skin.box);
            texturePainter.Mode = DrawEnumToolbar(texturePainter.Mode);
            texturePainter.BrushRadius = DrawSlider("Radius", texturePainter.BrushRadius, 1f, 64f);
            texturePainter.BrushOpacity = DrawSlider("Opacity", texturePainter.BrushOpacity, 0.01f, 1f);
            texturePainter.BrushTargetStrength = DrawSlider("Target", texturePainter.BrushTargetStrength, 0.01f, 1f);
            texturePainter.BrushRotation = DrawSlider("Rotation", texturePainter.BrushRotation, 0f, 360f);

            if (texturePainter.Mode == GTexturePaintingMode.Splat)
            {
                texturePainter.SelectedSplatIndex = DrawSelectionGrid("Splat Layer", texturePainter.SelectedSplatIndex, GetSplatNames());
            }
            else if (texturePainter.Mode == GTexturePaintingMode.Albedo ||
                texturePainter.Mode == GTexturePaintingMode.Metallic ||
                texturePainter.Mode == GTexturePaintingMode.Smoothness ||
                texturePainter.Mode == GTexturePaintingMode.Mask)
            {
                texturePainter.BrushColor = DrawColorFields(texturePainter.BrushColor);
            }
        }

        private void DrawFoliageToolGui()
        {
            GUILayout.Label("Foliage Tool", GUI.skin.box);
            foliagePainter.Mode = DrawEnumToolbar(foliagePainter.Mode);
            foliagePainter.BrushRadius = DrawSlider("Radius", foliagePainter.BrushRadius, 1f, 64f);
            foliagePainter.BrushDensity = DrawIntSlider("Density", foliagePainter.BrushDensity, 1, 24);
            foliagePainter.BrushRotation = DrawSlider("Rotation", foliagePainter.BrushRotation, 0f, 360f);
            foliagePainter.EraseRatio = DrawSlider("Erase Ratio", foliagePainter.EraseRatio, 0.05f, 1f);
            foliagePainter.ScaleStrength = DrawSlider("Scale Strength", foliagePainter.ScaleStrength, 0.1f, 3f);

            if (foliagePainter.Mode == GFoliagePaintingMode.PaintTree || foliagePainter.Mode == GFoliagePaintingMode.ScaleTree)
            {
                foliagePainter.SelectedTreeIndex = DrawSelectionGrid("Tree Prototype", foliagePainter.SelectedTreeIndex, GetTreeNames());
            }
            else
            {
                foliagePainter.SelectedGrassIndex = DrawSelectionGrid("Grass Prototype", foliagePainter.SelectedGrassIndex, GetGrassNames());
            }
        }

        private void DrawObjectToolGui()
        {
            GUILayout.Label("Object Tool", GUI.skin.box);
            objectPainter.Mode = DrawEnumToolbar(objectPainter.Mode);
            objectPainter.BrushRadius = DrawSlider("Radius", objectPainter.BrushRadius, 1f, 64f);
            objectPainter.BrushDensity = DrawIntSlider("Density", objectPainter.BrushDensity, 1, 16);
            objectPainter.BrushRotation = DrawSlider("Rotation", objectPainter.BrushRotation, 0f, 360f);
            objectPainter.EraseRatio = DrawSlider("Erase Ratio", objectPainter.EraseRatio, 0.05f, 1f);
            objectPainter.ScaleStrength = DrawSlider("Scale Strength", objectPainter.ScaleStrength, 0.1f, 3f);

            int selected = objectPainter.SelectedPrototypeIndices.Count > 0 ? objectPainter.SelectedPrototypeIndices[0] : 0;
            selected = DrawSelectionGrid("Object Prototype", selected, GetObjectNames());
            objectPainter.SelectedPrototypeIndices = new List<int> { selected };
        }

        private float DrawSlider(string label, float value, float min, float max)
        {
            GUILayout.Label(string.Format("{0}: {1:0.00}", label, value));
            return GUILayout.HorizontalSlider(value, min, max);
        }

        private int DrawIntSlider(string label, int value, int min, int max)
        {
            GUILayout.Label(string.Format("{0}: {1}", label, value));
            return Mathf.RoundToInt(GUILayout.HorizontalSlider(value, min, max));
        }

        private int DrawSelectionGrid(string label, int selected, string[] options)
        {
            GUILayout.Label(label);
            if (options == null || options.Length == 0)
            {
                GUILayout.Label("No options");
                return 0;
            }
            return GUILayout.SelectionGrid(Mathf.Clamp(selected, 0, options.Length - 1), options, 2);
        }

        private T DrawEnumToolbar<T>(T value)
        {
            System.Array values = System.Enum.GetValues(typeof(T));
            string[] labels = System.Enum.GetNames(typeof(T));
            int selectedIndex = System.Array.IndexOf(values, value);
            int nextIndex = GUILayout.Toolbar(selectedIndex, labels);
            return (T)values.GetValue(Mathf.Clamp(nextIndex, 0, labels.Length - 1));
        }

        private Color DrawColorFields(Color color)
        {
            GUILayout.Label(string.Format("Color: {0:0.00}, {1:0.00}, {2:0.00}, {3:0.00}", color.r, color.g, color.b, color.a));
            color.r = GUILayout.HorizontalSlider(color.r, 0f, 1f);
            color.g = GUILayout.HorizontalSlider(color.g, 0f, 1f);
            color.b = GUILayout.HorizontalSlider(color.b, 0f, 1f);
            color.a = GUILayout.HorizontalSlider(color.a, 0f, 1f);
            return color;
        }

        private string[] GetSplatNames()
        {
            if (runtimeSplatGroup == null)
                return new string[0];

            string[] names = new string[runtimeSplatGroup.Prototypes.Count];
            for (int i = 0; i < names.Length; ++i)
            {
                Texture2D tex = runtimeSplatGroup.Prototypes[i].Texture;
                names[i] = tex != null ? tex.name : string.Format("Layer {0}", i);
            }
            return names;
        }

        private string[] GetTreeNames()
        {
            if (runtimeTreeGroup == null)
                return new string[0];

            string[] names = new string[runtimeTreeGroup.Prototypes.Count];
            for (int i = 0; i < names.Length; ++i)
            {
                names[i] = runtimeTreeGroup.Prototypes[i].Prefab != null ? runtimeTreeGroup.Prototypes[i].Prefab.name : string.Format("Tree {0}", i);
            }
            return names;
        }

        private string[] GetGrassNames()
        {
            if (runtimeGrassGroup == null)
                return new string[0];

            string[] names = new string[runtimeGrassGroup.Prototypes.Count];
            for (int i = 0; i < names.Length; ++i)
            {
                names[i] = runtimeGrassGroup.Prototypes[i].Prefab != null ? runtimeGrassGroup.Prototypes[i].Prefab.name : string.Format("Grass {0}", i);
            }
            return names;
        }

        private string[] GetObjectNames()
        {
            string[] names = new string[generatedObjectPrefabs.Count];
            for (int i = 0; i < names.Length; ++i)
            {
                names[i] = generatedObjectPrefabs[i] != null ? generatedObjectPrefabs[i].name : string.Format("Object {0}", i);
            }
            return names;
        }

        private int GetActiveBrushMaskIndex()
        {
            if (paintCategory == GPaintCategory.Texture)
                return texturePainter.SelectedBrushMaskIndex;
            if (paintCategory == GPaintCategory.Foliage)
                return foliagePainter.SelectedBrushMaskIndex;
            return objectPainter.SelectedBrushMaskIndex;
        }

        private void SetActiveBrushMaskIndex(int index)
        {
            if (paintCategory == GPaintCategory.Texture)
                texturePainter.SelectedBrushMaskIndex = index;
            else if (paintCategory == GPaintCategory.Foliage)
                foliagePainter.SelectedBrushMaskIndex = index;
            else
                objectPainter.SelectedBrushMaskIndex = index;
        }

        private float GetActiveBrushRadius()
        {
            if (paintCategory == GPaintCategory.Texture)
                return texturePainter.BrushRadius;
            if (paintCategory == GPaintCategory.Foliage)
                return foliagePainter.BrushRadius;
            return objectPainter.BrushRadius;
        }

        private Vector2 GetGuiMousePosition()
        {
            Vector3 mouse = Input.mousePosition;
            return new Vector2(mouse.x, Screen.height - mouse.y);
        }

        private bool IsCtrlPressed()
        {
            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }

        private bool IsShiftPressed()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }

        private Vector3 GetTerrainCenterWorld()
        {
            return terrain.transform.position + new Vector3(terrainSize.x * 0.5f, terrainSize.y * 0.35f, terrainSize.z * 0.5f);
        }

        private Texture2D CreateBrushTexture(string textureName, int resolution, float falloffPower, bool square)
        {
            Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false, true);
            tex.name = textureName;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < resolution; ++y)
            {
                for (int x = 0; x < resolution; ++x)
                {
                    float u = (x + 0.5f) / resolution * 2f - 1f;
                    float v = (y + 0.5f) / resolution * 2f - 1f;
                    float value = square ? 1f : Mathf.Clamp01(1f - Mathf.Pow(Mathf.Sqrt(u * u + v * v), falloffPower));
                    tex.SetPixel(x, y, new Color(value, value, value, value));
                }
            }

            tex.Apply();
            RegisterRuntimeAsset(tex);
            return tex;
        }

        private Texture2D CreateCheckerTexture(string textureName, Color a, Color b)
        {
            Texture2D tex = new Texture2D(128, 128, TextureFormat.RGBA32, false, true);
            tex.name = textureName;
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < tex.height; ++y)
            {
                for (int x = 0; x < tex.width; ++x)
                {
                    bool odd = ((x / 16) + (y / 16)) % 2 == 0;
                    tex.SetPixel(x, y, odd ? a : b);
                }
            }

            tex.Apply();
            RegisterRuntimeAsset(tex);
            generatedSplatTextures.Add(tex);
            return tex;
        }

        private GameObject CreateTreeTemplate(string prefabName, Color trunkColor, Color leavesColor, float heightScale)
        {
            GameObject root = new GameObject(prefabName);
            root.transform.SetParent(templateRoot.transform, false);
            RegisterRuntimeAsset(root);

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(root.transform, false);
            trunk.transform.localPosition = new Vector3(0f, heightScale * 0.5f, 0f);
            trunk.transform.localScale = new Vector3(0.18f, heightScale * 0.5f, 0.18f);
            SetupTemplateRenderer(trunk, trunkColor);

            GameObject canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            canopy.name = "Canopy";
            canopy.transform.SetParent(root.transform, false);
            canopy.transform.localPosition = new Vector3(0f, heightScale * 1.35f, 0f);
            canopy.transform.localScale = Vector3.one * heightScale * 0.8f;
            SetupTemplateRenderer(canopy, leavesColor);

            CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0f, heightScale, 0f);
            collider.height = heightScale * 2f;
            collider.radius = heightScale * 0.25f;
            generatedTreePrefabs.Add(root);
            return root;
        }

        private GameObject CreateGrassTemplate(string prefabName, Color color, Vector3 scale)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Quad);
            root.name = prefabName;
            root.transform.SetParent(templateRoot.transform, false);
            root.transform.localScale = scale;
            RegisterRuntimeAsset(root);
            SetupTemplateRenderer(root, color);
            generatedGrassPrefabs.Add(root);
            return root;
        }

        private GameObject CreateRockTemplate(string prefabName, PrimitiveType primitiveType, Color color, Vector3 scale)
        {
            GameObject root = GameObject.CreatePrimitive(primitiveType);
            root.name = prefabName;
            root.transform.SetParent(templateRoot.transform, false);
            root.transform.localScale = scale;
            RegisterRuntimeAsset(root);
            SetupTemplateRenderer(root, color);
            generatedObjectPrefabs.Add(root);
            return root;
        }

        private void SetupTemplateRenderer(GameObject g, Color color)
        {
            Collider col = g.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }

            MeshRenderer renderer = g.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material mat = CreateOpaqueMaterial(color);
                renderer.sharedMaterial = mat;
                renderer.shadowCastingMode = ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }
        }

        private Material CreateOpaqueMaterial(Color color)
        {
            Shader shader =
                Shader.Find("Universal Render Pipeline/Lit") ??
                Shader.Find("Universal Render Pipeline/Simple Lit") ??
                Shader.Find("Standard") ??
                Shader.Find("Diffuse") ??
                Shader.Find("Sprites/Default");

            Material mat = new Material(shader);
            ApplyMaterialColor(mat, color);
            RegisterRuntimeAsset(mat);
            return mat;
        }

        private Material CreateCursorMaterial()
        {
            Shader shader =
                Shader.Find("Universal Render Pipeline/Unlit") ??
                Shader.Find("Unlit/Color") ??
                Shader.Find("Sprites/Default") ??
                Shader.Find("Standard");

            Material mat = new Material(shader);
            Color c = new Color(0.3f, 0.8f, 1f, 0.18f);
            ApplyMaterialColor(mat, c);

            if (mat.HasProperty("_Surface"))
            {
                mat.SetFloat("_Surface", 1f);
            }
            if (mat.HasProperty("_Blend"))
            {
                mat.SetFloat("_Blend", 0f);
            }
            if (mat.HasProperty("_SrcBlend"))
            {
                mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            }
            if (mat.HasProperty("_DstBlend"))
            {
                mat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            }
            if (mat.HasProperty("_ZWrite"))
            {
                mat.SetFloat("_ZWrite", 0f);
            }
            mat.renderQueue = (int)RenderQueue.Transparent;
            RegisterRuntimeAsset(mat);
            return mat;
        }

        private void ApplyMaterialColor(Material mat, Color color)
        {
            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", color);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", color);
            }
        }

        private void RegisterRuntimeAsset(Object o)
        {
            if (o != null && !runtimeAssets.Contains(o))
            {
                runtimeAssets.Add(o);
            }
        }

        private void CleanupRuntimeAssets()
        {
            for (int i = runtimeAssets.Count - 1; i >= 0; --i)
            {
                Object o = runtimeAssets[i];
                if (o == null)
                    continue;

                if (Application.isPlaying)
                {
                    Destroy(o);
                }
                else
                {
                    DestroyImmediate(o);
                }
            }

            runtimeAssets.Clear();
            terrain = null;
            texturePainter = null;
            foliagePainter = null;
            objectPainter = null;
            runtimeSplatGroup = null;
            runtimeTreeGroup = null;
            runtimeGrassGroup = null;
            templateRoot = null;
            cursorSphere = null;
            runtimeCamera = null;
            runtimeLight = null;
            appliedShadingMode = (GTexturingModel)(-1);
            hasCursorHit = false;
            wasHoldingPaint = false;
            hasLastPaintHit = false;
        }
    }
}
#endif
