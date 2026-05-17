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
    public class MaterialPage : IWizardPage
    {
        private class PageGUI
        {
            public static readonly string DESCRIPTION = "Select the default material for new terrain. You can change the terrain material at anytime.";
            public static readonly GUIContent APPLY = new GUIContent("Apply");
        }

        public string Path => "Wizard/Material";
        public string Title => "Material";

        public void OnGUI(GWizardWindow hostWindow)
        {
            EditorGUILayout.LabelField(PageGUI.DESCRIPTION);
            MaterialSelector.Draw();
        }
    }
}
#endif
