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
    public class JupiterPreviewPage : IWizardPage
    {
        public static event Func<bool> IsJupiterInstalled;
        public static event Func<string> GetJupiterVersionString;
        public static event Func<GameObject> GetJupiterSkyGameObjectInScene;
        public static event Func<GameObject> AddJupiterSky;
        public static event Action<GameObject, JupiterSkyPreview> MatchJupiterSettingsWithPreview;

        public string Path => "Wizard/JupiterPreview";
        public string Title => "Add animated sky";

        private class PageGUI
        {
            public static readonly Texture2D BANNER = GEditorSkin.Instance.GetTexture("JupiterBanner");
            public static readonly string CTA_LINK = GAssetLink.JUPITER;
        }

        public void OnGUI(GWizardWindow hostWindow)
        {
            DrawBodyContent(hostWindow);
        }

        private void DrawBodyContent(GWizardWindow hostWindow)
        {
            JupiterSkyPreview jupiterPreview = Object.FindFirstObjectByType<JupiterSkyPreview>();
            if (!IsJupiterInstalled())
            {
                if (jupiterPreview == null)
                {
                    EditorGUILayout.LabelField("The appearance of terrain is strongly influenced by lighting and sky color, especially in stylized and low-poly scenes.", GStyles.P1);
                    EditorGUILayout.LabelField("This preview adds a lightweight animated sky to help you evaluate terrain shapes, colors, and silhouettes under different lighting conditions.", GStyles.P1);
                    EditorGUILayout.LabelField("The preview is provided for visual evaluation only and does not affect terrain data.", GStyles.P1);
                    EditorGUILayout.Space();

                    if (GUILayout.Button("Add preview sky"))
                    {
                        GNetUtils.TrackClick("add_preview_sky", GUILocation.Wizard);

                        GameObject g = new GameObject("Day Sky - Jupiter Preview");
                        JupiterSkyPreview skyComponent = g.AddComponent<JupiterSkyPreview>();
                        skyComponent.ApplyPresetBasedOnDominantLight();
                        Selection.activeObject = skyComponent;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("A preview sky is already added to the scene.", GStyles.P1);
                    EditorGUILayout.ObjectField(jupiterPreview, typeof(JupiterSkyPreview), true);
                }
            }
            else
            {
                EditorGUILayout.LabelField($"<b>Jupiter {GetJupiterVersionString()} was installed.</b>", GStyles.P1);
                GameObject jupiterSkyObject = GetJupiterSkyGameObjectInScene();
                if (jupiterSkyObject == null && jupiterPreview == null)
                {
                    EditorGUILayout.LabelField($"Current scene doesn't contain Jupiter sky object.", GStyles.P1);
                    if (GUILayout.Button("Add Jupiter sky"))
                    {
                        jupiterSkyObject = AddJupiterSky();
                    }
                }
                else if (jupiterSkyObject == null && jupiterPreview != null && jupiterPreview.isActiveAndEnabled)
                {
                    EditorGUILayout.LabelField($"Current scene doesn't contain Jupiter sky object, but a preview is in used.", GStyles.P1);
                    if (GUILayout.Button("Add Jupiter sky & disable preview"))
                    {
                        jupiterPreview.gameObject.SetActive(false);
                        jupiterSkyObject = AddJupiterSky();
                        MatchJupiterSettingsWithPreview(jupiterSkyObject, jupiterPreview);
                    }
                }
                else if (jupiterSkyObject != null && jupiterPreview != null && jupiterPreview.isActiveAndEnabled)
                {
                    EditorGUILayout.LabelField($"Jupiter sky and its preview should not be active at the same time in the scene.", GStyles.P1);
                    if (GUILayout.Button("Disable preview"))
                    {
                        jupiterPreview.gameObject.SetActive(false);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Use the following object to configure your sky.", GStyles.P1);
                    EditorGUILayout.ObjectField(jupiterSkyObject, typeof(GameObject), true);
                }
            }
        }
    }
}
#endif
