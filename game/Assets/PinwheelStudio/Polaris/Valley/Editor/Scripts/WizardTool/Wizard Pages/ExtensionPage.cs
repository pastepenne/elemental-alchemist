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
    public class ExtensionPage : IWizardPage
    {
        public string Path => "Wizard/Extension";

        public string Title => "Extensions";

        private class PageGUI
        {

        }

        public void OnGUI(GWizardWindow hostWindow)
        {
            GExtensionTabDrawer.Draw();
        }
    }
}
#endif
