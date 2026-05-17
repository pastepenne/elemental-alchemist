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
    public class DataLocationPage : IWizardPage
    {
        public string Path => "Wizard/Data Location";

        public string Title => "Data Location";

        private class PageGUI
        {
            public static readonly GUIContent INSTRUCTION = new GUIContent("Select a folder to store your new terrain data assets, terrain meshes and materials.");
            public static readonly GUIContent DIRECTORY = new GUIContent("Directory", "Where to store created terrain data. A sub-folder of Assets/ is recommended.");
        }

        public void OnGUI(GWizardWindow hostWindow)
        {
            GEditorSettings.WizardToolsSettings settings = GEditorSettings.Instance.wizardTools;
            EditorGUILayout.LabelField(PageGUI.INSTRUCTION);
            EditorGUI.BeginChangeCheck();
            string dir = settings.dataDirectory;
            GEditorCommon.BrowseFolder(PageGUI.DIRECTORY, ref dir, true);
            if (string.IsNullOrEmpty(dir))
            {
                dir = "Assets/";
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(GEditorSettings.Instance, "Change wizard settings");
                EditorUtility.SetDirty(GEditorSettings.Instance);
                settings.dataDirectory = dir;
            }
        }
    }
}
#endif
