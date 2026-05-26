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
    public class AdditionalSettingsPage : IWizardPage
    {
        public string Path => "Wizard/Additional Settings";

        public string Title => "Additional Settings";

        private class PageGUI
        {
            public static readonly GUIContent NAME_PREFIX = new GUIContent("Name Prefix", "The beginning of each terrain's name. Useful for some level streaming system.");
            public static readonly GUIContent GROUP_ID = new GUIContent("Group Id", "An integer for grouping and connecting adjacent terrain tiles.");
        }

        public void OnGUI(GWizardWindow hostWindow)
        {
            GEditorSettings.WizardToolsSettings settings = GEditorSettings.Instance.wizardTools;
            EditorGUI.BeginChangeCheck();
            string namePrefix = EditorGUILayout.TextField(PageGUI.NAME_PREFIX, settings.terrainNamePrefix);
            int groupId = EditorGUILayout.IntField(PageGUI.GROUP_ID, settings.groupId);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(GEditorSettings.Instance, "Change wizard settings");
                EditorUtility.SetDirty(GEditorSettings.Instance);
                settings.terrainNamePrefix = namePrefix;
                settings.groupId = groupId;
            }
        }
    }
}
#endif
