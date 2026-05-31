#if GRIFFIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.Griffin
{
    public class GDebugWindow : EditorWindow
    {
        public GStylizedTerrain terrain;
        public int meshRes;
        public int pixelCount;

        [MenuItem("Window/Polaris/Debug")]
        public static void ShowWindow()
        {
            GDebugWindow window = GetWindow<GDebugWindow>();
            window.titleContent = new GUIContent("GDebugWindow");
            window.Show();
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);

            terrain = EditorGUILayout.ObjectField("Terrain", terrain, typeof(GStylizedTerrain), true) as GStylizedTerrain;

            if (terrain != null)
            {
                meshRes = EditorGUILayout.IntField("Res", meshRes);
                if (GUILayout.Button("Set Mesh Res"))
                {
                    terrain.TerrainData.Geometry.MeshResolution = meshRes;
                    terrain.TerrainData.Geometry.MeshBaseResolution = meshRes;
                    terrain.TerrainData.Geometry.SetRegionDirty(new Rect(0, 0, 1, 1));
                    terrain.TerrainData.SetDirty(GTerrainData.DirtyFlags.Geometry);

                    pixelCount = GGeometry.GetPixelCountByMeshResolutionForSingleChunk(meshRes);
                }

                EditorGUILayout.LabelField("Pixel Count", pixelCount.ToString());
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif
