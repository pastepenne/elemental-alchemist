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
    public class ContourPage : AssetAdPage
    {
        public override string Path => "Wizard/Contour";
        public override string Title => "Contour";

        public override Texture2D BANNER => PageGUI.BANNER;
        public override string HEADING => PageGUI.HEADING;
        public override string SUBHEADING => PageGUI.SUBHEADING;
        public override string CTA => PageGUI.CTA;
        public override string CTA_LINK => PageGUI.CTA_LINK;

        private class PageGUI
        {
            public static readonly Texture2D BANNER = GEditorSkin.Instance.GetTexture("ContourBanner");
            public static readonly string HEADING = "Edge Detection & Outline";
            public static readonly string SUBHEADING = "Make your visual aesthetic with Contour, a fullscreen post FX that adds subtle outline around objects.";
            public static readonly string CTA = "View on Asset Store";
            public static readonly string CTA_LINK = GAssetLink.CONTOUR;
        }
    }
}
#endif
