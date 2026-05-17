#if GRIFFIN_3
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.Griffin.PoseidonPreview;
using System.Linq;
#if POSEIDON_2
using Pinwheel.Poseidon;
#endif

namespace Pinwheel.Griffin
{
    public static class PoseidonProxy
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            Pinwheel.Griffin.Wizard.PoseidonPreviewPage.IsPoseidonInstalled += IsPoseidonInstalled;
            Pinwheel.Griffin.Wizard.PoseidonPreviewPage.GetPoseidonVersionString += GetPoseidonVersionString;
            Pinwheel.Griffin.Wizard.PoseidonPreviewPage.GetPoseidonGameObjectsInScene += GetPoseidonGameObjectsInScene;
            Pinwheel.Griffin.Wizard.PoseidonPreviewPage.AddTileableWater += AddTileableWater;
            Pinwheel.Griffin.Wizard.PoseidonPreviewPage.AddAreaWater += AddAreaWater;
            Pinwheel.Griffin.Wizard.PoseidonPreviewPage.AddRiverWater += AddRiverWater;
            Pinwheel.Griffin.Wizard.PoseidonPreviewPage.ConvertToPoseidonWater += ConvertToPoseidonWater;
        }

        public static bool IsPoseidonInstalled()
        {
#if POSEIDON_2
            return true;
#else
            return false;
#endif
        }

        public static string GetPoseidonVersionString()
        {
#if POSEIDON_2
            return PVersionInfo.Code;
#else
            return "";
#endif
        }

        public static GameObject[] GetPoseidonGameObjectsInScene()
        {
#if POSEIDON_2
            PoseidonWaterBody[] waters = Object.FindObjectsByType<PoseidonWaterBody>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            return waters.Select(w => w.gameObject).ToArray();
#else
            return new GameObject[0];
#endif
        }

        public static GameObject AddTileableWater()
        {
#if POSEIDON_2
            GameObject g = new GameObject("Tileable Water");
            TileableWater water = g.AddComponent<TileableWater>();
            return g;
#else
            return null;
#endif
        }

        public static GameObject AddAreaWater()
        {
#if POSEIDON_2
            GameObject g = new GameObject("Area Water");
            AreaWater water = g.AddComponent<AreaWater>();
            return g;
#else
            return null;
#endif
        }

        public static GameObject AddRiverWater()
        {
#if POSEIDON_2
            GameObject g = new GameObject("River Water");
            RiverWater water = g.AddComponent<RiverWater>();
            return g;
#else
            return null;
#endif
        }

        public static GameObject ConvertToPoseidonWater(PoseidonAreaWaterPreview previewWater)
        {
#if POSEIDON_2
            GameObject g = new GameObject("Poseidon Water - Converted from preview");
            AreaWater water = g.AddComponent<AreaWater>();

            water.material = Object.Instantiate(previewWater.material);
            water.material.name = "Poseidon material - converted";

            const string POSEIDON_SHADER_NAME = "Poseidon/ShaderGraph/7_Poly_Flat_Perf_PoseidonWater";
            Shader poseidonShader = Shader.Find(POSEIDON_SHADER_NAME);
            if (poseidonShader != null)
            {
                water.material.shader = poseidonShader;
            }
            else
            {
                Debug.Log($"Could not find shader {POSEIDON_SHADER_NAME} for converting to Poseidon water. This object will keep using the preview water shader.");
            }

            water.timeMode = (Poseidon.TimeMode)((int)previewWater.timeMode);
            water.manualTimeSeconds = previewWater.manualTimeSeconds;

            Poseidon.AreaMeshDesc meshDesc = new Poseidon.AreaMeshDesc()
            {
                resolution = previewWater.meshDesc.resolution,
                needNormals = false,
                needTangents = false
            };
            water.meshDesc = meshDesc;

            water.anchors.Clear();
            water.anchors.AddRange(previewWater.anchors);
            water.GenerateMesh();

            return g;
#else
            return null;
#endif
        }
    }
}

#endif