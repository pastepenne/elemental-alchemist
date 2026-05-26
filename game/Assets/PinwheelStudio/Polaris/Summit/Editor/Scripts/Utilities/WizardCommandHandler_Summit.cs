#if GRIFFIN && !GRIFFIN_EXCLUDE_SUMMIT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using Pinwheel.Griffin.ErosionTool;

namespace Pinwheel.Griffin.Wizard.Summit
{
    [InitializeOnLoad]
    public static class WizardCommandHandler
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            MainPage.AddErosionSimulatorToSceneCallback += OnAddErosionSimulatorToScene;
        }

        private static void OnAddErosionSimulatorToScene()
        {
            GameObject root = GWizard.GetTerrainToolsRoot();
            if (root == null)
            {
                root = GWizard.CreateTerrainToolsRoot();
            }
            GameObject g = new GameObject("Erosion Simulator");
            g.transform.parent = root.transform;
            g.transform.position = Vector3.zero;
            g.transform.rotation = Quaternion.identity;
            g.transform.localScale = Vector3.one;

            GErosionSimulator erosionSimulator = g.AddComponent<GErosionSimulator>();

            EditorGUIUtility.PingObject(erosionSimulator);
            Selection.activeGameObject = erosionSimulator.gameObject;
        }
    }
}
#endif