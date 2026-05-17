#if GRIFFIN && !GRIFFIN_EXCLUDE_SUMMIT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.Griffin.ErosionTool;
using UnityEditor;

namespace Pinwheel.Griffin
{
    public static class PolarisSummitEditor
    {
        [MenuItem("GameObject/3D Object/Polaris/Erosion Simulator", false, -699)]
        public static void CreateErosionSimulator(MenuCommand menuCmd)
        {
            GameObject simulatorGO = new GameObject("Erosion Simulator");
            if (menuCmd != null)
                GameObjectUtility.SetParentAndAlign(simulatorGO, menuCmd.context as GameObject);
            simulatorGO.transform.localPosition = Vector3.zero;
            simulatorGO.transform.localRotation = Quaternion.identity;
            simulatorGO.transform.localScale = Vector3.one * 100;
            GErosionSimulator simulator = simulatorGO.AddComponent<GErosionSimulator>();
            simulator.GroupId = -1;

            Selection.activeGameObject = simulatorGO;
            Undo.RegisterCreatedObjectUndo(simulatorGO, "Creating Erosion Simulator");
        }
    }
}
#endif