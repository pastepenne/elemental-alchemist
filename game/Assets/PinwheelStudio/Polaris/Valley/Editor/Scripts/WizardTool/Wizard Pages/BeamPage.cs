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
    public class BeamPage : AssetAdPage
    {
        public override string Path => "Wizard/Beam";
        public override string Title => "Beam";

        public override Texture2D BANNER => PageGUI.BANNER;
        public override string HEADING => PageGUI.HEADING;
        public override string SUBHEADING => PageGUI.SUBHEADING;
        public override string CTA => PageGUI.CTA;
        public override string CTA_LINK => PageGUI.CTA_LINK;

        private class PageGUI
        {
            public static readonly Texture2D BANNER = GEditorSkin.Instance.GetTexture("BeamBanner");
            public static readonly string HEADING = "Volumetric Lighting & Fog";
            public static readonly string SUBHEADING = "Next level lighting with volumetric sun shafts, halos and light beams.";
            public static readonly string CTA = "View on Asset Store";
            public static readonly string CTA_LINK = GAssetLink.BEAM;
        }
    }
}
#endif
