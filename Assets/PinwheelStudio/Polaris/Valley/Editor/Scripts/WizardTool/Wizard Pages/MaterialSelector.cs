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
    public static class MaterialSelector
    {
        private static class PageGUI
        {
            public static readonly string HEADER_LIGHTING_MODEL = "Lighting Model";
            public static readonly string PHYSICAL_BASED = "Physical Based";
            public static readonly string PHYSICAL_BASED_DESC = "Current-gen lighting method with physical properties.";
            public static readonly Texture2D PHYSICAL_BASED_IMAGE = GEditorSkin.Instance.GetTexture("PhysicalBasedThumbnail");

            public static readonly string LAMBERT = "Lambert";
            public static readonly string LAMBERT_DESC = "Simple diffuse lighting, mobile friendly.";
            public static readonly Texture2D LAMBERT_IMAGE = GEditorSkin.Instance.GetTexture("LambertThumbnail");

            public static readonly string BLINN_PHONG = "Blinn-Phong";
            public static readonly string BLINN_PHONG_DESC = "Simple specular lighting, mobile friendly.";
            public static readonly Texture2D BLINN_PHONG_IMAGE = GEditorSkin.Instance.GetTexture("BlinnPhongThumbnail");

            public static readonly string HEADER_TEXTURING_MODEL = "Texturing Model";
            public static readonly string SPLATS4 = "4 Splats";
            public static readonly string SPLATS4_DESC = "4 texture layers without normal maps blend together using painted weight. The 5th layer and up will appear black.";
            public static readonly Texture2D SPLATS4_IMAGE = GEditorSkin.Instance.GetTexture("Splats4Thumbnail");

            public static readonly string SPLATS4_NORMALS4 = "4 Splats with Normal Maps";
            public static readonly string SPLATS4_NORMALS4_DESC = "4 texture layers with normal maps blend together using painted weight. The 5th layer and up will appear black.";
            public static readonly Texture2D SPLATS4_NORMALS4_IMAGE = GEditorSkin.Instance.GetTexture("Splats4Normals4Thumbnail");

            public static readonly string SPLATS8 = "8 Splats";
            public static readonly string SPLATS8_DESC = "8 texture layers without normal maps blend together using painted weight. The 9th layer and up will appear black.";
            public static readonly Texture2D SPLATS8_IMAGE = GEditorSkin.Instance.GetTexture("Splats8Thumbnail");

            public static readonly string COLOR_MAP = "Color Map";
            public static readonly string COLOR_MAP_DESC = "A color map and a metallic/smoothness map cover the whole terrain.";
            public static readonly Texture2D COLOR_MAP_IMAGE = GEditorSkin.Instance.GetTexture("ColorMapThumbnail");

            public static readonly string GL = "Gradient Lookup";
            public static readonly string GL_DESC = "Procedural coloring with color by height and color by normal vectors, encoded in gradients, plus a color map for overlaying detail.";
            public static readonly Texture2D GL_IMAGE = GEditorSkin.Instance.GetTexture("GradientLookupThumbnail");

            public static readonly string VERTEX_COLOR = "Vertex Color";
            public static readonly string VERTEX_COLOR_DESC = "Colors are baked into the terrain mesh. Cheap to render but no live preview.";
            public static readonly Texture2D VERTEX_COLOR_IMAGE = GEditorSkin.Instance.GetTexture("VertexColorThumbnail");

        }

        public static void Draw()
        {
            GEditorSettings.WizardToolsSettings settings = GEditorSettings.Instance.wizardTools;

            EditorGUI.BeginChangeCheck();
            GEditorCommon.Header(PageGUI.HEADER_LIGHTING_MODEL);
            if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Universal)
            {
                settings.lightingModel = GLightingModel.PBR;
            }
            GLightingModel lightingModel = settings.lightingModel;

            EditorGUILayout.BeginHorizontal();
            if (GEditorCommon.SelectionCard(PageGUI.PHYSICAL_BASED_IMAGE, PageGUI.PHYSICAL_BASED, PageGUI.PHYSICAL_BASED_DESC, lightingModel == GLightingModel.PBR))
            {
                lightingModel = GLightingModel.PBR;
            }
            if (GCommon.CurrentRenderPipeline != GRenderPipelineType.Universal)
            {
                if (GEditorCommon.SelectionCard(PageGUI.LAMBERT_IMAGE, PageGUI.LAMBERT, PageGUI.LAMBERT_DESC, lightingModel == GLightingModel.Lambert))
                {
                    lightingModel = GLightingModel.Lambert;
                }
                if (GEditorCommon.SelectionCard(PageGUI.BLINN_PHONG_IMAGE, PageGUI.BLINN_PHONG, PageGUI.BLINN_PHONG_DESC, lightingModel == GLightingModel.BlinnPhong))
                {
                    lightingModel = GLightingModel.BlinnPhong;
                }
            }
            EditorGUILayout.EndHorizontal();

            GEditorCommon.Space();

            GEditorCommon.Header(PageGUI.HEADER_TEXTURING_MODEL);
            GTexturingModel texturingModel = settings.texturingModel;
            GSplatsModel splatsModel = settings.splatsModel;
            EditorGUILayout.BeginHorizontal();
            if (GEditorCommon.SelectionCard(PageGUI.SPLATS4_IMAGE, PageGUI.SPLATS4, PageGUI.SPLATS4_DESC, texturingModel == GTexturingModel.Splat && splatsModel == GSplatsModel.Splats4, true))
            {
                texturingModel = GTexturingModel.Splat;
                splatsModel = GSplatsModel.Splats4;
            }
            if (GEditorCommon.SelectionCard(PageGUI.SPLATS4_NORMALS4_IMAGE, PageGUI.SPLATS4_NORMALS4, PageGUI.SPLATS4_NORMALS4_DESC, texturingModel == GTexturingModel.Splat && splatsModel == GSplatsModel.Splats4Normals4, true))
            {
                texturingModel = GTexturingModel.Splat;
                splatsModel = GSplatsModel.Splats4Normals4;
            }
            if (GEditorCommon.SelectionCard(PageGUI.SPLATS8_IMAGE, PageGUI.SPLATS8, PageGUI.SPLATS8_DESC, texturingModel == GTexturingModel.Splat && splatsModel == GSplatsModel.Splats8, true))
            {
                texturingModel = GTexturingModel.Splat;
                splatsModel = GSplatsModel.Splats8;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GEditorCommon.SelectionCard(PageGUI.COLOR_MAP_IMAGE, PageGUI.COLOR_MAP, PageGUI.COLOR_MAP_DESC, texturingModel == GTexturingModel.ColorMap, true))
            {
                texturingModel = GTexturingModel.ColorMap;
            }
            if (GEditorCommon.SelectionCard(PageGUI.GL_IMAGE, PageGUI.GL, PageGUI.GL_DESC, texturingModel == GTexturingModel.GradientLookup, true))
            {
                texturingModel = GTexturingModel.GradientLookup;
            }
            if (GEditorCommon.SelectionCard(PageGUI.VERTEX_COLOR_IMAGE, PageGUI.VERTEX_COLOR, PageGUI.VERTEX_COLOR_DESC, texturingModel == GTexturingModel.VertexColor, true))
            {
                texturingModel = GTexturingModel.VertexColor;
            }
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(GEditorSettings.Instance, "Change wizard settings");
                EditorUtility.SetDirty(GEditorSettings.Instance);
                settings.lightingModel = lightingModel;
                settings.texturingModel = texturingModel;
                settings.splatsModel = splatsModel;
            }
        }
    }
}
#endif
