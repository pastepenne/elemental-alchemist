#if GRIFFIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Pinwheel.Griffin.GroupTool;
using Pinwheel.Griffin.PaintTool;
using Pinwheel.Griffin.TextureTool;
using System;
using Pinwheel.Griffin.JupiterPreview;
using Pinwheel.Griffin.PoseidonPreview;
using Object = UnityEngine.Object;

namespace Pinwheel.Griffin.Wizard
{
    public class SetMaterialPage : IWizardPage
    {
        public string Path => "Wizard/Set Material";
        public string Title => "Set Material";
        public bool bulkMode = false;

        private class PageGUI
        {
            public static readonly string DESCRIPTION = "Select new lighting & texturing mode for your terrain.";
            public static readonly string HEADER_TARGET = "Target";
            public static readonly GUIContent GROUP_ID = new GUIContent("Group Id", "Id of the terrain group to change the material");
            public static readonly GUIContent TERRAIN = new GUIContent("Terrain", "The terrain to change its material");

            public static readonly GUIContent APPLY_BTN = new GUIContent("Apply");
        }

        public void OnGUI(GWizardWindow hostWindow)
        {
            GEditorSettings.WizardToolsSettings settings = GEditorSettings.Instance.wizardTools;
            EditorGUILayout.LabelField(PageGUI.DESCRIPTION);
            EditorGUI.BeginChangeCheck();
            GEditorCommon.Header(PageGUI.HEADER_TARGET);
            int groupId = settings.setShaderGroupId;
            GStylizedTerrain terrain = settings.setShaderTerrain;
            if (bulkMode)
            {
                groupId = GEditorCommon.ActiveTerrainGroupPopupWithAllOption(PageGUI.GROUP_ID, settings.setShaderGroupId);
            }
            else
            {
                terrain = EditorGUILayout.ObjectField(PageGUI.TERRAIN, settings.setShaderTerrain, typeof(GStylizedTerrain), true) as GStylizedTerrain;
            }
            MaterialSelector.Draw();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(GEditorSettings.Instance, "Change Wizard settings");
                EditorUtility.SetDirty(GEditorSettings.Instance);
                settings.setShaderGroupId = groupId;
                settings.setShaderTerrain = terrain;
            }
        }

        public void OnTitleRightGUILayout(GWizardWindow hostWindow)
        {
            if (GUILayout.Button(PageGUI.APPLY_BTN, GUILayout.Width(100)))
            {
                GEditorSettings.WizardToolsSettings settings = GEditorSettings.Instance.wizardTools;
                if (bulkMode)
                {
                    GWizard.SetShader(settings.setShaderGroupId);
                }
                else
                {
                    GWizard.SetShader(settings.setShaderTerrain);
                }
            }
        }
    }
}
#endif
