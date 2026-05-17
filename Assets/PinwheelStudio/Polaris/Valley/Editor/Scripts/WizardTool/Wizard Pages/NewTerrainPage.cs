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
    public class NewTerrainPage : IWizardPage
    {
        private class PageGUI
        {
            public static readonly GUIContent ORIGIN = new GUIContent("Origin", "Position of the first terrain in the grid.");
            public static readonly GUIContent TILE_SIZE = new GUIContent("Tile Size", "Size of each terrain tile in world space.");
            public static readonly GUIContent TILE_WIDTH = new GUIContent("Tile Width", "Width of each terrain tile in world space.");
            public static readonly GUIContent TILE_LENGTH = new GUIContent("Tile Length", "Length of each terrain tile in world space.");
            public static readonly GUIContent TILE_MAX_HEIGHT = new GUIContent("Tile Height", "Maximum height of each terrain tile in world space.");
            public static readonly GUIContent TILE_X = new GUIContent("Tile Count X", "Number of tiles along X-axis.");
            public static readonly GUIContent TILE_Z = new GUIContent("Tile Count Z", "Number of tiles along Z-axis.");
            public static readonly GUIContent CREATE_BTN = new GUIContent("Create");

            public static readonly GUIContent TERRAIN_MATERIAL = new GUIContent("Terrain Material");
            public static readonly GUIContent DATA_LOCATION = new GUIContent("Data Location");
            public static readonly GUIContent ADDITIONAL_SETTINGS = new GUIContent("Additional Settings");

            public static readonly Texture2D CONTEXT_ICON = GEditorSkin.Instance.GetTexture("ContextButtonIcon");

            public static readonly string TUT_KEY_SET_MATERIAL = "polaris-wizard-create-set-mat";
            public static readonly GUIContent TUT_SET_MATERIAL = new GUIContent("Set terrain material and other settings →");

            public static GUIContent MATERIAL_SELECTION = new GUIContent("");

            private static GUIStyle shadingInfoStyle;
            public static GUIStyle ShadingInfoStyle
            {
                get
                {
                    if (shadingInfoStyle == null)
                    {
                        shadingInfoStyle = new GUIStyle(EditorStyles.miniButton);
                        shadingInfoStyle.fixedHeight = 18;
                        shadingInfoStyle.stretchWidth = false;
                    }
                    return shadingInfoStyle;
                }
            }

        }

        public string Path => "Wizard/New Terrain";

        public string Title => "New Terrain";

        public void OnGUI(GWizardWindow hostWindow)
        {
            GEditorSettings.WizardToolsSettings settings = GEditorSettings.Instance.wizardTools;
            float spacing = 2;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            int originalIndent = EditorGUI.indentLevel;

            #region pre-register spaces
            EditorGUILayout.GetControlRect(); //Tile Count X label
            EditorGUILayout.GetControlRect(); //Tile Count X field
            Rect rectTerrainGridBase = EditorGUILayout.GetControlRect(GUILayout.Height(lineHeight * 15));
            EditorGUILayout.GetControlRect(); //Origin field
            EditorGUILayout.GetControlRect(); //Tile Width field
            EditorGUILayout.GetControlRect(); //Tile Length field
            EditorGUILayout.GetControlRect(); //Tile Height field
            EditorGUILayout.Space();
            #endregion

            EditorGUI.BeginChangeCheck();
            Handles.BeginGUI();

            EditorGUIUtility.labelWidth = 70;
            EditorGUI.indentLevel = 0;

            #region Draw terrain grid & measurements
            Vector3 worldSize = new Vector3
            (
                settings.tileCountX * settings.tileSize.x,
                settings.tileSize.y,
                settings.tileCountZ * settings.tileSize.z
            );

            float worldAspect = worldSize.x / worldSize.z;
            float rectTerrainGridBaseSize = rectTerrainGridBase.height;
            Vector2 rectTerrainGridSize = new Vector2(rectTerrainGridBaseSize * worldAspect, rectTerrainGridBaseSize);
            Vector2 rectTerrainGridCenter = rectTerrainGridBase.center - new Vector2(0, lineHeight * 0);
            Rect rectTerrainGrid = new Rect()
            {
                size = rectTerrainGridSize,
                center = rectTerrainGridCenter
            };

            float rectTerrainGridScaleFactor = Mathf.Min(1, rectTerrainGridBase.width / rectTerrainGrid.width);
            rectTerrainGrid.size *= rectTerrainGridScaleFactor;
            rectTerrainGrid.center = rectTerrainGridCenter;

            Handles.DrawSolidRectangleWithOutline(rectTerrainGrid, Color.clear, Color.gray);
            for (int z = 0; z < settings.tileCountZ; ++z)
            {
                for (int x = 0; x < settings.tileCountX; ++x)
                {
                    Vector2 rectTileSize = new Vector2(rectTerrainGrid.width / settings.tileCountX, rectTerrainGrid.height / settings.tileCountZ);
                    Vector2 rectTilePosition = new Vector2(rectTerrainGrid.min.x + x * rectTileSize.x, rectTerrainGrid.min.y + z * rectTileSize.y);
                    Rect rectTile = new Rect()
                    {
                        size = rectTileSize,
                        position = rectTilePosition
                    };

                    Handles.DrawSolidRectangleWithOutline(rectTile, Color.clear, Color.gray);
                }
            }

            if (rectTerrainGrid.width > 50 || rectTerrainGrid.height > 50)
            {
                Handles.color = Handles.xAxisColor;
                Handles.DrawLine(
                    new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y),
                    new Vector3(rectTerrainGrid.max.x, rectTerrainGrid.max.y),
                    1);
                Rect rectWorldWidth = new Rect()
                {
                    size = new Vector2(100, lineHeight),
                    center = new Vector2(rectTerrainGrid.center.x, rectTerrainGrid.max.y - lineHeight * 0.5f)
                };
                GUI.contentColor = Handles.xAxisColor * 1.2f;
                EditorGUI.LabelField(rectWorldWidth, $"{worldSize.x}m", GEditorCommon.CenteredBoldLabel);

                Handles.color = Handles.zAxisColor;
                Handles.DrawLine(
                    new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.min.y),
                    new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y),
                    1);
                Rect rectWorldLength = new Rect()
                {
                    size = new Vector2(100, lineHeight),
                    position = new Vector2(rectTerrainGrid.min.x + spacing, rectTerrainGrid.center.y - lineHeight * 0.5f)
                };
                GUI.contentColor = Handles.zAxisColor * 1.2f;
                EditorGUI.LabelField(rectWorldLength, $"{worldSize.z}m", GEditorCommon.BoldLabel);
            }
            #endregion

            #region Fields below terrain grid
            GUI.contentColor = Color.white;
            Rect rectOriginField = new Rect()
            {
                size = new Vector2(Mathf.Clamp(rectTerrainGrid.width, 250, 300), lineHeight),
                position = new Vector2(Mathf.Max(rectTerrainGridBase.min.x, rectTerrainGrid.min.x), rectTerrainGrid.max.y + spacing * 3)
            };
            Rect rectTileWidthField = new Rect()
            {
                size = rectOriginField.size,
                position = new Vector2(rectOriginField.min.x, rectOriginField.max.y + spacing)
            };
            Rect rectTileLengthField = new Rect()
            {
                size = rectOriginField.size,
                position = new Vector2(rectTileWidthField.min.x, rectTileWidthField.max.y + spacing)
            };
            Rect rectTileHeightField = new Rect()
            {
                size = rectOriginField.size,
                position = new Vector2(rectTileLengthField.min.x, rectTileLengthField.max.y + spacing)
            };

            EditorGUIUtility.wideMode = true;
            const string ORIGIN_FIELD_NAME = "OriginField";
            GUI.SetNextControlName(ORIGIN_FIELD_NAME);
            Vector3 origin = EditorGUI.Vector3Field(rectOriginField, PageGUI.ORIGIN, settings.origin);
            const string TILE_WIDTH_FIELD_NAME = "TileWidthField";
            GUI.SetNextControlName(TILE_WIDTH_FIELD_NAME);
            float tileWidth = EditorGUI.DelayedFloatField(rectTileWidthField, PageGUI.TILE_WIDTH, settings.tileSize.x);
            GUI.enabled = !settings.linkTileSize;
            const string TILE_LENGTH_FIELD_NAME = "TileLengthField";
            GUI.SetNextControlName(TILE_LENGTH_FIELD_NAME);
            float tileLength = EditorGUI.DelayedFloatField(rectTileLengthField, PageGUI.TILE_LENGTH, settings.tileSize.z);
            GUI.enabled = true;
            float tileHeight = EditorGUI.DelayedFloatField(rectTileHeightField, PageGUI.TILE_MAX_HEIGHT, settings.tileSize.y);

            if (string.Equals(ORIGIN_FIELD_NAME, GUI.GetNameOfFocusedControl()) ||
                rectOriginField.Contains(Event.current.mousePosition))
            {
                Handles.color = new Color(1, 1, 1, 0.7f);
                Handles.DrawSolidDisc(
                    new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y),
                    Vector3.forward, 4);
            }

            if (string.Equals(TILE_WIDTH_FIELD_NAME, GUI.GetNameOfFocusedControl()) ||
                rectTileWidthField.Contains(Event.current.mousePosition))
            {
                Handles.color = new Color(1, 1, 1, 0.7f);
                Handles.DrawLine(
                    new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y),
                    new Vector3(rectTerrainGrid.min.x + rectTerrainGrid.width / settings.tileCountX, rectTerrainGrid.max.y),
                    1);
            }

            if (string.Equals(TILE_LENGTH_FIELD_NAME, GUI.GetNameOfFocusedControl()) ||
              rectTileLengthField.Contains(Event.current.mousePosition))
            {
                Handles.color = new Color(1, 1, 1, 0.7f);
                Handles.DrawLine(
                    new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y),
                    new Vector3(rectTerrainGrid.min.x, rectTerrainGrid.max.y - rectTerrainGrid.height / settings.tileCountZ),
                    1);
            }

            #region Link tile size
            Vector3[] linkTileSizeLine = new Vector3[]
            {
                    new Vector2(rectTileWidthField.max.x + spacing, rectTileWidthField.min.y),
                    new Vector2(rectTileWidthField.max.x + spacing*6, rectTileWidthField.min.y),
                    new Vector2(rectTileLengthField.max.x + spacing*6, rectTileLengthField.max.y),
                    new Vector2(rectTileLengthField.max.x + spacing, rectTileLengthField.max.y),
            };
            int[] linkTileSizeLineIndices = new int[] { 0, 1, 1, 2, 2, 3 };
            Handles.color = Color.gray;
            Handles.DrawLines(linkTileSizeLine, linkTileSizeLineIndices);

            Rect linkTileSizeButtonRect = new Rect()
            {
                size = Vector2.one * lineHeight,
                center = (linkTileSizeLine[1] + linkTileSizeLine[2]) * 0.5f
            };
            Texture2D linkIcon = null;
            if (settings.linkTileSize)
            {
                linkIcon = GEditorSkin.Instance.GetTexture("ChainIcon");
            }
            else
            {
                linkIcon = GEditorSkin.Instance.GetTexture("ChainBreakIcon");
            }
            bool linkTileSize = settings.linkTileSize;
            GUI.color = EditorStyles.label.normal.textColor;
            if (GUI.Button(linkTileSizeButtonRect, linkIcon, EditorStyles.iconButton))
            {
                linkTileSize = !linkTileSize;
            }
            if (linkTileSize)
            {
                tileLength = tileWidth;
            }
            GUI.color = Color.white;
            #endregion
            #endregion

            #region Tile Count fields
            float tileCountFieldWidth = 72;
            Rect rectTileCountZField = new Rect()
            {
                size = new Vector2(tileCountFieldWidth, lineHeight),
                position = new Vector2(Mathf.Max(rectTerrainGridBase.min.x, rectTerrainGrid.min.x), rectTerrainGrid.min.y - lineHeight - spacing * 2)
            };
            Rect rectTileCountZLabel = new Rect()
            {
                size = new Vector2(tileCountFieldWidth, lineHeight),
                position = new Vector2(rectTileCountZField.min.x, rectTileCountZField.min.y - lineHeight)
            };
            EditorGUI.LabelField(rectTileCountZLabel, PageGUI.TILE_Z);
            int tileCountZ = EditorGUI.DelayedIntField(rectTileCountZField, settings.tileCountZ);

            Rect rectTileCountXField = new Rect()
            {
                size = new Vector2(tileCountFieldWidth, lineHeight),
                position = new Vector2(Mathf.Min(rectTerrainGridBase.max.x - tileCountFieldWidth, rectTerrainGrid.max.x + spacing * 2), rectTerrainGrid.max.y - lineHeight)
            };
            Rect rectTileCountXLabel = new Rect()
            {
                size = new Vector2(tileCountFieldWidth, lineHeight),
                position = new Vector2(rectTileCountXField.min.x, rectTileCountXField.min.y - lineHeight)
            };
            EditorGUI.LabelField(rectTileCountXLabel, PageGUI.TILE_X);
            int tileCountX = EditorGUI.DelayedIntField(rectTileCountXField, settings.tileCountX);
            #endregion

            EditorGUIUtility.wideMode = false;
            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUI.indentLevel = originalIndent;
            Handles.EndGUI();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(GEditorSettings.Instance, "Change terrain wizard settings");
                EditorUtility.SetDirty(GEditorSettings.Instance);
                settings.origin = origin;
                settings.tileSize = new Vector3(
                    Mathf.Max(1, tileWidth),
                    Mathf.Max(1, tileHeight),
                    Mathf.Max(1, tileLength));
                settings.tileCountX = Mathf.Max(1, tileCountX);
                settings.tileCountZ = Mathf.Max(1, tileCountZ);
                settings.linkTileSize = linkTileSize;
            }
        }

        public void OnTitleRightGUILayout(GWizardWindow hostWindow)
        {
            GEditorSettings.WizardToolsSettings wizardSettings = GEditorSettings.Instance.wizardTools;
            string shadingInfoText;
            if (wizardSettings.texturingModel == GTexturingModel.Splat)
            {
                shadingInfoText = $"  <i>{wizardSettings.lightingModel} {wizardSettings.splatsModel}</i>  ";
            }
            else
            {
                shadingInfoText = $"  <i>{wizardSettings.lightingModel} {wizardSettings.texturingModel}</i>  ";
            }
            PageGUI.MATERIAL_SELECTION.text = shadingInfoText;

            GUIStyle shadingInfoStyle = GEditorCommon.IconButton;
            Vector2 shadingInfoGUISize = shadingInfoStyle.CalcSize(PageGUI.MATERIAL_SELECTION);
            Rect shadingInfoRect = EditorGUILayout.GetControlRect(GUILayout.Width(shadingInfoGUISize.x));
            EditorGUIUtility.AddCursorRect(shadingInfoRect, MouseCursor.Link);

            if (GUI.Button(shadingInfoRect, PageGUI.MATERIAL_SELECTION, shadingInfoStyle))
            {
                PopupWindow.Show(shadingInfoRect, new MaterialSelectorPopup());
            }

            Rect contextButtonRect = EditorGUILayout.GetControlRect(GUILayout.Width(EditorGUIUtility.singleLineHeight), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            GUI.contentColor = EditorStyles.label.normal.textColor;
            if (GUI.Button(contextButtonRect, PageGUI.CONTEXT_ICON, GEditorCommon.IconButton))
            {
                EditorPrefs.SetBool(PageGUI.TUT_KEY_SET_MATERIAL, true);
                GenericMenu menu = new GenericMenu();
                menu.AddItem(
                    PageGUI.TERRAIN_MATERIAL, false, () =>
                    {
                        PopupWindow.Show(contextButtonRect, new MaterialSelectorPopup());
                    });
                menu.AddItem(
                    PageGUI.DATA_LOCATION, false, () =>
                    {
                        PopupWindow.Show(contextButtonRect, new DataLocationPopup(hostWindow));
                    });
                menu.AddItem(
                    PageGUI.ADDITIONAL_SETTINGS, false, () =>
                    {
                        PopupWindow.Show(contextButtonRect, new AdditionalSettingsPopup(hostWindow));
                    });
                menu.ShowAsContext();
            }
            GUI.contentColor = Color.white;
            if (GUILayout.Button(PageGUI.CREATE_BTN, GUILayout.Width(100)))
            {
                GameObject environmentRoot = new GameObject("Low Poly Environment");
                environmentRoot.transform.position = wizardSettings.origin;

                GWizard.CreateTerrains(environmentRoot);
            }
            EditorGUILayout.GetControlRect(GUILayout.Width(4));
        }

        class MaterialSelectorPopup : PopupWindowContent
        {
            Vector2 scrollPos;

            public override Vector2 GetWindowSize()
            {
                return new Vector2(550, 600);
            }

            public override void OnGUI(Rect rect)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GEditorCommon.WindowBodyStyle);
                MaterialSelector.Draw();
                EditorGUILayout.EndScrollView();
            }
        }

        class DataLocationPopup : PopupWindowContent
        {
            DataLocationPage page;
            GWizardWindow wizardWindow;

            public DataLocationPopup(GWizardWindow wizardWindow)
            {
                this.page = new DataLocationPage();
                this.wizardWindow = wizardWindow;
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(500, 54);
            }

            public override void OnGUI(Rect rect)
            {
                EditorGUILayout.BeginVertical(GEditorCommon.WindowBodyStyle);
                EditorGUILayout.Space();
                page.OnGUI(wizardWindow);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }

        class AdditionalSettingsPopup : PopupWindowContent
        {
            AdditionalSettingsPage page;
            GWizardWindow wizardWindow;

            public AdditionalSettingsPopup(GWizardWindow wizardWindow)
            {
                this.page = new AdditionalSettingsPage();
                this.wizardWindow = wizardWindow;
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(500, 54);
            }

            public override void OnGUI(Rect rect)
            {
                EditorGUILayout.BeginVertical(GEditorCommon.WindowBodyStyle);
                EditorGUILayout.Space();
                page.OnGUI(wizardWindow);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif
