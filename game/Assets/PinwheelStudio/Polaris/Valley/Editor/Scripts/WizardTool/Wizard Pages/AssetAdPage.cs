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
    public abstract class AssetAdPage : IWizardPage
    {
        public abstract string Path { get; }
        public abstract string Title { get; }
        public abstract Texture2D BANNER { get; }
        public abstract string HEADING { get; }
        public abstract string SUBHEADING { get; }
        public abstract string CTA { get; }
        public abstract string CTA_LINK { get; }

        private static GUIStyle bannerStyle;
        private static GUIStyle BannerStyle
        {
            get
            {
                if (bannerStyle == null)
                {
                    bannerStyle = new GUIStyle();
                    bannerStyle.padding = new RectOffset(24, 0, 0, 0);
                }
                return bannerStyle;
            }
        }

        public void OnGUI(GWizardWindow hostWindow)
        {
            Rect bannerRect = EditorGUILayout.BeginVertical(BannerStyle);
            GUI.DrawTexture(bannerRect, BANNER, ScaleMode.ScaleAndCrop);
            EditorGUILayout.GetControlRect(GUILayout.Height(64));
            EditorGUILayout.LabelField(HEADING, GStyles.H1, GUILayout.Width(400));
            EditorGUILayout.LabelField(SUBHEADING, GStyles.P1, GUILayout.Width(400));
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(CTA, GUILayout.Width(150), GUILayout.Height(24)))
            {
                string trackingButtonName = $"{Title.ToLower()}_{ObjectNames.NicifyVariableName(CTA).ToLower().Replace(' ', '_')}";
                GNetUtils.TrackClick(trackingButtonName, GUILocation.Wizard);

                string url = GNetUtils.ModURL(CTA_LINK, "", GAssetLink.UTM_SOURCE, $"wizard-{GetType().Name}");
                Application.OpenURL(url);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.GetControlRect(GUILayout.Height(64));
            EditorGUILayout.EndVertical();
        }
    }
}
#endif
