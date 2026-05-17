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
    public interface IWizardPage
    {
        string Path { get; }
        string Title { get; }
        //string Description => string.Empty;
        void OnTitleGUI(GWizardWindow hostWindow, Rect titleRect) { }
        void OnTitleRightGUILayout(GWizardWindow hostWindow) { }
        void OnGUI(GWizardWindow hostWindow) { }
        void OnPush(GWizardWindow hostWindow) { }
        bool OnPop(GWizardWindow hostWindow) { return true; }
    }
}
#endif
