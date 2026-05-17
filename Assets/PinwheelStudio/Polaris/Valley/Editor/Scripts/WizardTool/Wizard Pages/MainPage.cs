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
    public class MainPage : IWizardPage
    {
        public string Path => "Home";

        public string Title
        {
            get
            {
                if (GPackageInitializer.isPolarisSummitInstalled)
                {
                    return "Polaris Summit";
                }
                else if (GPackageInitializer.isPolarisHighlandInstalled)
                {
                    return "Polaris Highland";
                }
                else
                {
                    return "Polaris Valley";
                }
            }
        }

        private class PageGUI
        {
            public static readonly Texture2D PACKAGE_ICON = GEditorSkin.Instance.GetTexture("PackageIcon");
            public static readonly Texture2D PRO_FEATURE_ICON = GEditorSkin.Instance.GetTexture("IconWhite");

            public static readonly string HEADER_NEW_TERRAIN = "New Terrain";
            public static readonly GUIContent CREATE = new GUIContent("Create", "Add new terrains.");
            public static readonly GUIContent TERRAIN_MATERIAL = new GUIContent("Terrain Material", "Select default lighting & texturing mode.");
            public static readonly GUIContent DATA_LOCATION = new GUIContent("Data Location", "Select a folder for terrain data.");
            public static readonly GUIContent ADDITIONAL_SETTINGS = new GUIContent("Additional Settings", "Utility variables.");

            public static readonly string HEADER_ADD_TO_SCENE = "Add to Scene";
            public static readonly GUIContent GROUP_TOOL = new GUIContent("Group Tool", "Change settings of many terrains at once.");
            public static readonly GUIContent PAINTERS = new GUIContent("Painters", "Paint height, textures, vegetations and objects with brushes.");
            public static readonly GUIContent SPLINE = new GUIContent("Spline", "Create ramps, paint road texture, spawn & remove vegetations and objects.");
            public static readonly GUIContent SPLINE_LOCKED = new GUIContent("Spline", PRO_FEATURE_ICON, "Create ramps, paint road texture, spawn & remove vegetations and objects.");
            public static readonly GUIContent STAMPERS = new GUIContent("Stampers", "Stamp height maps and spawn objects procedurally.");
            public static readonly GUIContent STAMPERS_LOCKED = new GUIContent("Stampers", PRO_FEATURE_ICON, "Stamp height maps and spawn objects procedurally.");
            public static readonly GUIContent EROSION_SIMULATOR = new GUIContent("Erosion Simulator", "Apply thermal & hydraulic erosion on terrain geometry & textures.");
            public static readonly GUIContent EROSION_SIMULATOR_LOCKED = new GUIContent("Erosion Simulator", PRO_FEATURE_ICON, "Apply thermal & hydraulic erosion on terrain geometry & textures.");
            public static readonly GUIContent WATER = new GUIContent("Water", "Add a lightweight low poly water for decoration.");
            public static readonly GUIContent WIND = new GUIContent("Wind", "Adjust wind direction & intensity for grass waving.");
            public static readonly GUIContent PROC_TERRAIN = new GUIContent("Procedural Terrain", PACKAGE_ICON, "Generate your scene with graphs & biomes.");

            public static readonly string HEADER_SCENE_ENHANCEMENT = "Scene Enhancement";
            public static readonly GUIContent SKY = new GUIContent("Sky", "Add animated sky with rolling cloud.");
            public static readonly GUIContent OUTLINE_FX = new GUIContent("Outline FX", PACKAGE_ICON, "Fullscreen post effect that adds subtle outline to objects.");
            public static readonly GUIContent VOLUMETRIC_LIGHTING = new GUIContent("Volumetric Lighting", PACKAGE_ICON, "Realistic sun shafts, halos and light beams.");

            public static readonly string HEADER_UTILITIES = "Utilities";
            public static readonly GUIContent CHANGE_MATERIAL = new GUIContent("Set Terrain Material", "Change lighting & texturing mode of terrains.");
            public static readonly GUIContent NAV_BAKING = new GUIContent("Nav Mesh Helper", "Create dummy objects as nav mesh obstacles.");
            public static readonly GUIContent EXTRACT_TERRAIN_TEXTURE = new GUIContent("Extract Textures", "Extract terrain textures and other maps to files.");
            public static readonly GUIContent EXTENSIONS = new GUIContent("Extensions", "Additional functionalities from other assets.");

            public static readonly Texture2D BELL_ICON = GEditorSkin.Instance.GetTexture("NotificationIcon");
            public static readonly Texture2D EMAIL_ICON = GEditorSkin.Instance.GetTexture("EmailIcon");
            public static readonly Texture2D DOC_ICON = GEditorSkin.Instance.GetTexture("DocumentationIcon");
            public static readonly GUIContent CONTACT = new GUIContent(EMAIL_ICON, "Contact us");
            public static readonly GUIContent DOCUMENTATION = new GUIContent(DOC_ICON, "Open Documentation");

            public static readonly GUIContent PACKAGE_ICON_LEGEND = new GUIContent("Features from other products", PageGUI.PACKAGE_ICON);
            public static readonly GUIContent PRO_FEATURES_ICON_LEGEND = new GUIContent("Additional gears from Highland or Summit edition", PageGUI.PRO_FEATURE_ICON);
            public static readonly GUIContent COMPARE_EDITION = new GUIContent("Compare editions");
        }

        internal static event Action AddErosionSimulatorToSceneCallback;
        internal static event Action AddSplineToSceneCallback;
        internal static event Action AddStampersToSceneCallback;

        public void OnGUI(GWizardWindow hostWindow)
        {
            EditorGUILayout.LabelField(PageGUI.HEADER_NEW_TERRAIN, GStyles.H3);
            EditorGUILayout.BeginHorizontal();
            if (Tile(PageGUI.CREATE, DrawOverlay_CreateTerrain))
            {
                hostWindow.PushPage(new NewTerrainPage());
            }
            if (Tile(PageGUI.TERRAIN_MATERIAL))
            {
                hostWindow.PushPage(new MaterialPage());
            }
            if (Tile(PageGUI.DATA_LOCATION))
            {
                hostWindow.PushPage(new DataLocationPage());
            }
            if (Tile(PageGUI.ADDITIONAL_SETTINGS))
            {
                hostWindow.PushPage(new AdditionalSettingsPage());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(PageGUI.HEADER_ADD_TO_SCENE, GStyles.H3);
            EditorGUILayout.BeginHorizontal();
            if (Tile(PageGUI.GROUP_TOOL))
            {
                GTerrainGroup group = GWizard.CreateGroupTool();
                EditorGUIUtility.PingObject(group);
                Selection.activeGameObject = group.gameObject;
            }
            if (Tile(PageGUI.PAINTERS))
            {
                GTerrainTexturePainter texPainter = GWizard.CreateGeometryTexturePainter();
                GFoliagePainter foliagePainter = GWizard.CreateFoliagePainter();
                GObjectPainter objectPainter = GWizard.CreateObjectPainter();
                EditorGUIUtility.PingObject(texPainter);
                Selection.activeGameObject = texPainter.gameObject;
            }

            if (GPackageInitializer.isPolarisHighlandInstalled)
            {
                if (Tile(PageGUI.SPLINE))
                {
                    AddSplineToSceneCallback?.Invoke();
                }
            }
            else
            {
                GUI.enabled = false;
                Tile(PageGUI.SPLINE_LOCKED);
                GUI.enabled = true;
            }

            if (GPackageInitializer.isPolarisHighlandInstalled)
            {
                if (Tile(PageGUI.STAMPERS))
                {
                    AddStampersToSceneCallback?.Invoke();
                }
            }
            else
            {
                GUI.enabled = false;
                Tile(PageGUI.STAMPERS_LOCKED);
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GPackageInitializer.isPolarisSummitInstalled)
            {
                if (Tile(PageGUI.EROSION_SIMULATOR))
                {
                    AddErosionSimulatorToSceneCallback?.Invoke();
                }
            }
            else
            {
                GUI.enabled = false;
                Tile(PageGUI.EROSION_SIMULATOR_LOCKED);
                GUI.enabled = true;
            }

            if (Tile(PageGUI.WATER, null))
            {
                hostWindow.PushPage(new PoseidonPreviewPage());
            }

            if (Tile(PageGUI.WIND))
            {
                GWindZone windZone = GWizard.CreateWindZone();
                EditorGUIUtility.PingObject(windZone);
                Selection.activeGameObject = windZone.gameObject;
            }

            if (Tile(PageGUI.PROC_TERRAIN, null, DrawBackground_Vista))
            {
                hostWindow.PushPage(new VistaPage());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(PageGUI.HEADER_SCENE_ENHANCEMENT, GStyles.H3);
            EditorGUILayout.BeginHorizontal();
            if (Tile(PageGUI.SKY, null))
            {
                hostWindow.PushPage(new JupiterPreviewPage());
            }
            if (Tile(PageGUI.OUTLINE_FX, null, DrawBackground_Contour))
            {
                hostWindow.PushPage(new ContourPage());
            }
            if (Tile(PageGUI.VOLUMETRIC_LIGHTING, null, DrawBackground_Beam))
            {
                hostWindow.PushPage(new BeamPage());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(PageGUI.HEADER_UTILITIES, GStyles.H3);
            EditorGUILayout.BeginHorizontal();
            if (Tile(PageGUI.CHANGE_MATERIAL))
            {
                hostWindow.PushPage(new SetMaterialPage());
            }
            if (Tile(PageGUI.NAV_BAKING))
            {
                GNavigationHelper navHelper = GWizard.CreateNavHelper();
                EditorGUIUtility.PingObject(navHelper);
                Selection.activeGameObject = navHelper.gameObject;
            }
            if (Tile(PageGUI.EXTRACT_TERRAIN_TEXTURE))
            {
                GTextureEditorWindow.ShowWindow();
            }
            if (Tile(PageGUI.EXTENSIONS))
            {
                hostWindow.PushPage(new ExtensionPage());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (!GPackageInitializer.isPolarisSummitInstalled)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.color = GStyles.P2.normal.textColor;
                EditorGUILayout.LabelField(PageGUI.PRO_FEATURES_ICON_LEGEND, GUILayout.Width(300));
                GUI.color = Color.white;
                Rect compareEditionRect = EditorGUILayout.GetControlRect(GUILayout.Width(96));
                EditorGUIUtility.AddCursorRect(compareEditionRect, MouseCursor.Link);
                GUI.color = new Color32(135, 185, 250, 255);
                if (GUI.Button(compareEditionRect, PageGUI.COMPARE_EDITION, EditorStyles.miniLabel))
                {
                    GNetUtils.TrackClick("compare_editions", GUILocation.Wizard);
                    Application.OpenURL(GAssetLink.POLARIS_COMPARE_EDITIONS);
                }
                GUI.color = Color.white;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            GUI.color = GStyles.P2.normal.textColor;
            EditorGUILayout.LabelField(PageGUI.PACKAGE_ICON_LEGEND);
            GUI.color = Color.white;
        }

        private void DrawOverlay_CreateTerrain(Rect tileRect)
        {
            float animF = 0;
            if (tileRect.Contains(Event.current.mousePosition))
            {
                System.DateTime time = System.DateTime.Now;
                float f = time.Ticks % 20000000;
                f = f / 20000000f;
                f = Mathf.Sin(f * Mathf.PI * 2) * 0.5f + 0.5f;
                animF = f;
            }

            Rect r = new Rect();
            r.size = new Vector2(30, 30);
            r.position = new Vector2(tileRect.max.x - r.size.x - 8, tileRect.max.y - r.size.y - 8 - animF * 6);

            float plusThickness = 3;
            Rect rPlusH = new Rect();
            rPlusH.size = new Vector2(r.width * 0.5f, plusThickness);
            rPlusH.center = r.center;

            Rect rPlusV = new Rect();
            rPlusV.size = new Vector2(plusThickness, r.height * 0.5f);
            rPlusV.center = r.center;

            Color plusColor = GEditorCommon.midGrey;
            EditorGUI.DrawRect(rPlusH, plusColor);
            EditorGUI.DrawRect(rPlusV, plusColor);
            Handles.BeginGUI();
            Handles.color = plusColor;
            Handles.DrawWireDisc(r.center, Vector3.forward, r.size.x * 0.5f);
            Handles.EndGUI();
            Handles.color = Color.white;
        }

        private void DrawTextureOverlayOnHover(Rect tileRect, string textureName)
        {
            if (tileRect.Contains(Event.current.mousePosition))
            {
                Texture2D overlay = GEditorSkin.Instance.GetTexture(textureName);
                if (overlay != null)
                {
                    GUI.DrawTexture(tileRect, overlay, ScaleMode.ScaleAndCrop);
                }
            }
        }

        private void DrawBackground_Poseidon(Rect tileRect)
        {
            DrawTextureOverlayOnHover(tileRect, "PoseidonTileHover");
        }

        private void DrawBackground_Jupiter(Rect tileRect)
        {
            DrawTextureOverlayOnHover(tileRect, "JupiterTileHover");
        }

        private void DrawBackground_Vista(Rect tileRect)
        {
            DrawTextureOverlayOnHover(tileRect, "VistaTileHover");
        }

        private void DrawBackground_Contour(Rect tileRect)
        {
            DrawTextureOverlayOnHover(tileRect, "ContourTileHover");
        }

        private void DrawBackground_Beam(Rect tileRect)
        {
            DrawTextureOverlayOnHover(tileRect, "BeamTileHover");
        }

        private class TileGUI
        {
            public static readonly Vector2 tileSize = new Vector2(140, 80);
            public static readonly Color borderColor = GEditorCommon.midGrey;
            public static readonly Color backgroundColor = GEditorCommon.darkGrey;
            public static readonly Color hoverColor = new Color(1, 1, 1, 0.05f);
            public static readonly Color clickedColor = new Color(0, 0, 0, 0.2f);
            public static readonly Color disabledColor = new Color(0, 0, 0, 0.1f);

            public static Rect mouseTriggerRect = new Rect();
            public static bool isMouseTriggered = false;

            private static GUIStyle bodyStyle;
            public static GUIStyle BodyStyle
            {
                get
                {
                    if (bodyStyle == null)
                    {
                        bodyStyle = new GUIStyle();
                        bodyStyle.margin = new RectOffset(0, 8, 0, 8);
                        bodyStyle.padding = new RectOffset(8, 8, 8, 8);
                    }
                    return bodyStyle;
                }
            }
        }

        internal static bool Tile(GUIContent content, System.Action<Rect> overlayGUIDrawer = null, System.Action<Rect> backgroundDrawer = null)
        {
            Vector2 tileSize = TileGUI.tileSize;
            Rect r = EditorGUILayout.BeginVertical(TileGUI.BodyStyle, GUILayout.Width(tileSize.x), GUILayout.Height(tileSize.y));
            //if user left click on this tile, save it position and state for drawing highlight in latter GUI passes
            if (GUI.enabled &&
                Event.current.type == EventType.MouseDown &&
                Event.current.button == 0 &&
                r.Contains(Event.current.mousePosition))
            {
                TileGUI.mouseTriggerRect = r;
                TileGUI.isMouseTriggered = true;
            }

            //use release the left mouse button, reset 'active button' state
            if (Event.current.type == EventType.MouseUp &&
                Event.current.button == 0)
            {
                TileGUI.isMouseTriggered = false;
            }
            Handles.BeginGUI();
            GEditorCommon.DrawBodyBox(r, true);
            if (backgroundDrawer != null)
            {
                backgroundDrawer.Invoke(r);
            }
            EditorGUILayout.LabelField(content.text, GStyles.P1, GUILayout.Width(tileSize.x));
            EditorGUILayout.LabelField(content.tooltip, GStyles.P2, GUILayout.Width(tileSize.x));
            Handles.EndGUI();
            EditorGUILayout.EndVertical();

            if (content.image != null)
            {
                Rect iconRect = new Rect();
                iconRect.size = Vector2.one * EditorGUIUtility.singleLineHeight;
                iconRect.position = new(r.max.x - iconRect.width - 4, r.min.y + 4);
                GUI.color = GStyles.P2.normal.textColor;
                GUI.DrawTexture(iconRect, content.image);
                GUI.color = Color.white;
            }

            if (overlayGUIDrawer != null)
            {
                overlayGUIDrawer.Invoke(r);
            }

            //draw 'pressed' or 'hovered' highlight
            if (GUI.enabled && TileGUI.isMouseTriggered && TileGUI.mouseTriggerRect == r)
            {
                EditorGUI.DrawRect(r, TileGUI.clickedColor);
            }
            else if (GUI.enabled && !TileGUI.isMouseTriggered && r.Contains(Event.current.mousePosition))
            {
                EditorGUI.DrawRect(r, TileGUI.hoverColor);
            }

            //GEditorCommon.DrawOutlineBox(r, GUI.enabled ? Color.cyan : Color.red);
            if (!GUI.enabled)
            {
                EditorGUI.DrawRect(r, TileGUI.disabledColor);
            }

            //user release the left mouse on the active button
            if (GUI.enabled &&
                Event.current.type == EventType.MouseUp &&
                r.Contains(Event.current.mousePosition) &&
                r == TileGUI.mouseTriggerRect)
            {
                GUI.changed = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnTitleRightGUILayout(GWizardWindow hostWindow)
        {
            Rect contactRect = EditorGUILayout.GetControlRect(GUILayout.Width(20), GUILayout.Height(20));
            GUI.contentColor = EditorStyles.label.normal.textColor;
            if (GUI.Button(contactRect, PageGUI.CONTACT, GEditorCommon.IconButton))
            {
                Application.OpenURL(GCommon.CONTACT_PAGE);
            }

            Rect documentationRect = EditorGUILayout.GetControlRect(GUILayout.Width(20), GUILayout.Height(20));
            if (GUI.Button(documentationRect, PageGUI.DOCUMENTATION, GEditorCommon.IconButton))
            {
                Application.OpenURL(GCommon.ONLINE_MANUAL);
            }
            GUI.contentColor = Color.white;

            Rect notificationButtonRect = EditorGUILayout.GetControlRect(GUILayout.Width(20), GUILayout.Height(20));
            if (GUI.Button(notificationButtonRect, PageGUI.BELL_ICON, GEditorCommon.IconButton))
            {
                PopupWindow.Show(notificationButtonRect, new ExplorePopup());
            }

            EditorGUILayout.GetControlRect(GUILayout.Width(4));//add some spacing to the right
        }

        private class ExplorePopup : PopupWindowContent
        {
            public override void OnOpen()
            {
                base.OnOpen();
                EditorApplication.update += editorWindow.Repaint;
            }

            public override void OnClose()
            {
                base.OnClose();
                EditorApplication.update -= editorWindow.Repaint;
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(400, 600);
            }

            public override void OnGUI(Rect rect)
            {
                EditorGUILayout.BeginVertical(GEditorCommon.WindowBodyStyle);
                GExploreTabDrawer.Draw();
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif
