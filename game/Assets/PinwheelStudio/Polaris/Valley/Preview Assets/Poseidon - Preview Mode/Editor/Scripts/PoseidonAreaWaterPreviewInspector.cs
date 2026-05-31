#if GRIFFIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Overlays;

namespace Pinwheel.Griffin.PoseidonPreview
{
    [CustomEditor(typeof(PoseidonAreaWaterPreview))]
    public class PoseidonAreaWaterPreviewInspector : Editor
    {
        private PoseidonAreaWaterPreview instance;

        public static PoseidonAreaWaterPreview activeSelectedInstance;

        [SerializeField]
        private Texture2D poseidonBackground;
        [SerializeField]
        private Material defaultWaterMaterial;


        [SerializeField]
        protected Texture2D toolIcon;
        public static Texture2D editWaterAreaToolIcon;

        enum LearnMorePoseidonDrawPosition
        {
            None,
            MeshResolution
        }

        private LearnMorePoseidonDrawPosition learnMorePoseidonDrawPosition = LearnMorePoseidonDrawPosition.None;

        private static GUIStyle bannerNoteLabel;
        private static GUIStyle BannerNoteLabel
        {
            get
            {
                if (bannerNoteLabel == null)
                {
                    bannerNoteLabel = new GUIStyle(EditorStyles.miniLabel);
                }
                bannerNoteLabel.alignment = TextAnchor.LowerRight;
                bannerNoteLabel.fontStyle = FontStyle.Italic;
                return bannerNoteLabel;
            }
        }

        private void OnEnable()
        {
            instance = target as PoseidonAreaWaterPreview;
            editWaterAreaToolIcon = toolIcon;

            activeSelectedInstance = instance;
        }

        private void OnDisable()
        {
            activeSelectedInstance = null;
        }

        public override void OnInspectorGUI()
        {
            if (poseidonBackground != null)
            {
                Rect backgroundRect = EditorGUILayout.GetControlRect(GUILayout.Height(90));
                GUI.DrawTexture(backgroundRect, poseidonBackground, ScaleMode.ScaleAndCrop);

                GUI.Label(backgroundRect, "Low poly water made with Poseidon", BannerNoteLabel);
            }

            EditorGUILayout.Space();
            instance.material = EditorGUILayout.ObjectField("Material", instance.material, typeof(Material), false) as Material;

            EditorGUI.BeginChangeCheck();
            AreaMeshDesc meshDesc = instance.meshDesc;
            int meshRes = EditorGUILayout.IntSlider("Mesh Resolution", meshDesc.resolution, 2, 100);
            if (meshRes > AreaMeshDesc.MESH_RES_CAP)
            {
                learnMorePoseidonDrawPosition = LearnMorePoseidonDrawPosition.MeshResolution;
                meshRes = AreaMeshDesc.MESH_RES_CAP;
            }
            if (learnMorePoseidonDrawPosition == LearnMorePoseidonDrawPosition.MeshResolution)
            {
                DrawLearnMorePoseidonLink("Mesh resolution controls vertex density and surface coverage. The preview is capped at a reduced range for evaluation. Full Poseidon supports higher resolutions for larger water meshes.");
            }
            meshDesc.resolution = meshRes;
            instance.meshDesc = meshDesc;

            if (EditorGUI.EndChangeCheck())
            {
                instance.GenerateMesh();
            }

            instance.timeMode = (TimeMode)EditorGUILayout.EnumPopup("Time Mode", instance.timeMode);
            if (instance.timeMode == TimeMode.Manual)
            {
                instance.manualTimeSeconds = EditorGUILayout.FloatField("Time", instance.manualTimeSeconds);
            }

        }

        private void DrawLearnMorePoseidonLink(string message)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ");
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(message, EditorStyles.wordWrappedLabel);
            if (EditorGUILayout.LinkButton("Learn more about Poseidon →"))
            {
                NetUtils.TrackClick("learn_more_poseidon", UILocation.InspectorUtils);
                Application.OpenURL("https://assetstore.unity.com/packages/vfx/shaders/poseidon-2-low-poly-water-system-357908");
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

    [EditorTool("Edit Preview Water Area", typeof(PoseidonAreaWaterPreview))]
    public class EditWaterAreaTool : EditorTool
    {
        protected GUIContent m_toolbarIcon;
        public override GUIContent toolbarIcon
        {
            get
            {
                return m_toolbarIcon;
            }
        }

        private int m_selectedAnchorIndex;

        private void OnEnable()
        {

            m_toolbarIcon = new GUIContent(Resources.Load<Texture2D>("Polaris/PoseidonPreviewTextures/EditWaterAreaIcon"), "Edit water area");
        }

        private void OnDisable()
        {
            if (m_toolbarIcon != null && m_toolbarIcon.image != null)
            {
                Resources.UnloadAsset(m_toolbarIcon.image);
            }
        }

        public override void OnActivated()
        {
            m_selectedAnchorIndex = -1;
        }

        public override void OnWillBeDeactivated()
        {
            foreach (var obj in targets)
            {
                if ((obj is PoseidonAreaWaterPreview water) && water != null)
                {
                    water.GenerateMesh();
                }
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (!(window is SceneView sceneView))
                return;

            foreach (var obj in targets)
            {
                if (!(obj is PoseidonAreaWaterPreview water))
                    continue;

                if (water == null)
                    continue;

                HandleSelectTranslateRemoveAnchors(water);
                HandleAddAnchor(water);
                CatchHotControl();
            }
        }

        private void HandleSelectTranslateRemoveAnchors(PoseidonAreaWaterPreview water)
        {
            List<Vector3> localPositions = water.anchors;
            if (localPositions.Count == 0)
                return;
            if (localPositions.Count >= 2)
            {
                List<Vector3> worldPositions = new List<Vector3>();
                for (int i = 0; i < localPositions.Count; ++i)
                {
                    worldPositions.Add(water.transform.TransformPoint(localPositions[i]));
                }

                Handles.color = new Color(0, 1, 1, 1);
                Handles.DrawPolyLine(worldPositions.ToArray());
                Handles.color = new Color(0, 1, 1, 0.25f);
                Handles.DrawLine(worldPositions[0], worldPositions[worldPositions.Count - 1]);
            }

            for (int i = 0; i < localPositions.Count; ++i)
            {
                Vector3 localPos = localPositions[i];
                Vector3 worldPos = water.transform.TransformPoint(localPos);
                float handleSize = HandleUtility.GetHandleSize(worldPos) * 0.2f;
                if (i == m_selectedAnchorIndex)
                {
                    Handles.color = Handles.selectedColor;
                    Handles.SphereHandleCap(0, worldPos, Quaternion.identity, handleSize, EventType.Repaint);
                    worldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
                    localPos = water.transform.InverseTransformPoint(worldPos);
                    localPos.y = 0;
                    localPositions[i] = localPos;
                }
                else
                {
                    Handles.color = Color.cyan;
                    if (Handles.Button(worldPos, Quaternion.identity, handleSize, handleSize * 0.5f, Handles.SphereHandleCap))
                    {
                        if (Event.current.control)
                        {
                            m_selectedAnchorIndex = -1;
                            localPositions.RemoveAt(i);
                        }
                        else
                        {
                            m_selectedAnchorIndex = i;
                        }
                    }
                }
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                m_selectedAnchorIndex = -1;
            }
        }

        private void HandleAddAnchor(PoseidonAreaWaterPreview water)
        {
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                Plane plane = new Plane(Vector3.up, water.transform.position);
                Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                float distance = -1;
                if (plane.Raycast(r, out distance))
                {
                    Vector3 hitWorldPos = r.origin + r.direction * distance;
                    Vector3 hitLocalPos = water.transform.InverseTransformPoint(hitWorldPos);
                    if (Event.current.shift)
                    {
                        if (Event.current.control)
                        {
                            AnchorUtilities.Insert(water.anchors, hitLocalPos);
                            Event.current.Use();
                        }
                        else
                        {
                            water.anchors.Add(hitLocalPos);
                            Event.current.Use();
                        }
                    }
                }
            }
        }

        private void CatchHotControl()
        {
            int controlId = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    GUIUtility.hotControl = controlId;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                if (GUIUtility.hotControl == controlId)
                {
                    //Return the hot control back to Unity, use the default
                    GUIUtility.hotControl = 0;
                }
            }
        }
    }

    [Overlay(typeof(SceneView), "Edit preview water area")]
    public class EditWaterAreaOverlay : IMGUIOverlay, ITransientOverlay
    {
        public bool visible => ToolManager.activeToolType == typeof(EditWaterAreaTool);

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Click to select");
            EditorGUILayout.LabelField("Shift + Click to add anchor");
            EditorGUILayout.LabelField("Ctrl + Shift + Click to insert anchor");
            EditorGUILayout.LabelField("Ctrl + Click to remove anchor");
        }
    }

    [Overlay(typeof(SceneView), "Poseidon preview water tips")]
    public class LargeWaterAreaTipsOverlay : IMGUIOverlay, ITransientOverlay
    {
        private const float SIZE_LIMIT = 75;

        public bool visible
        {
            get
            {
                PoseidonAreaWaterPreview water = PoseidonAreaWaterPreviewInspector.activeSelectedInstance;
                if (water == null)
                    return false;

                Bounds b = WaterUtilities.GetAnchorBoundsWS(water);
                bool isExceedSizeLimit = b.size.x > SIZE_LIMIT || b.size.z > SIZE_LIMIT;
                return isExceedSizeLimit;
            }
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Large water bodies need additional setup", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("At this scale, visual quality depends on higher mesh resolution and extended tooling. The full Poseidon package provides options designed for larger water surfaces.", EditorStyles.wordWrappedLabel, GUILayout.Width(400));
            if (EditorGUILayout.LinkButton("Learn more about Poseidon →"))
            {
                NetUtils.TrackClick("learn_more_poseidon", UILocation.SceneOverlays);
                Application.OpenURL("https://assetstore.unity.com/packages/vfx/shaders/poseidon-2-low-poly-water-system-357908");
            }
        }
    }
}
#endif
