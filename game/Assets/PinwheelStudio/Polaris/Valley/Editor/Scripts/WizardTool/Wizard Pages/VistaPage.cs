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
    public class VistaPage : AssetAdPage
    {
        public override string Path => "Wizard/Vista";
        public override string Title => "Vista";

        public override Texture2D BANNER => PageGUI.BANNER;
        public override string HEADING => PageGUI.HEADING;
        public override string SUBHEADING => PageGUI.SUBHEADING;
        public override string CTA => PageGUI.CTA;
        public override string CTA_LINK => PageGUI.CTA_LINK;

        private class PageGUI
        {
            public static readonly Texture2D BANNER = GEditorSkin.Instance.GetTexture("VistaBanner");
            public static readonly string HEADING = "Procedural Terrain Generator";
            public static readonly string SUBHEADING = "Generate your terrain with Vista, a powerful graph tool with multi-biomes workflow.";
            public static readonly string CTA = "View on Asset Store";
            public static readonly string CTA_LINK = GAssetLink.VISTA_PRO;
        }
    }
}
#endif
