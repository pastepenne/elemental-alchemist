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
    public class PoseidonPreviewPage : IWizardPage
    {
        public static Func<bool> IsPoseidonInstalled;
        public static Func<string> GetPoseidonVersionString;
        public static Func<GameObject[]> GetPoseidonGameObjectsInScene;
        public static Func<GameObject> AddTileableWater;
        public static Func<GameObject> AddAreaWater;
        public static Func<GameObject> AddRiverWater;
        public static Func<PoseidonAreaWaterPreview, GameObject> ConvertToPoseidonWater;

        public string Path => "Wizard/PoseidonPreview";
        public string Title => "Add water";

        private class PageGUI
        {
            public static readonly Texture2D TILEABLE_WATER_IMAGE = GEditorSkin.Instance.GetTexture("PoseidonTileableWaterThumbnail");
            public static readonly Texture2D AREA_WATER_IMAGE = GEditorSkin.Instance.GetTexture("PoseidonAreaWaterThumbnail");
            public static readonly Texture2D RIVER_WATER_IMAGE = GEditorSkin.Instance.GetTexture("PoseidonRiverWaterThumbnail");
        }



        public void OnGUI(GWizardWindow hostWindow)
        {
            DrawAddNewWaterGUI();
            DrawPreviewWaterList();
            DrawPoseidonWaterList();
        }

        private void DrawAddNewWaterGUI()
        {
            EditorGUILayout.LabelField("Add new water body", GStyles.H3);
            if (!IsPoseidonInstalled())
            {
                EditorGUILayout.LabelField("Adds a lightweight water surface to help evaluate terrain shape and scene scale. This preview uses a subset of Poseidon’s water features for visual evaluation.", GStyles.P1);

                EditorGUILayout.BeginHorizontal();
                GUI.enabled = true;
                if (GEditorCommon.ClickableCard(PageGUI.AREA_WATER_IMAGE, "Area Water (Preview Mode)", "Use anchors to define water shape, suitable for medium to small water such as ponds, lakes, puddles."))
                {
                    GNetUtils.TrackClick("add_preview_water", GUILocation.Wizard);

                    GameObject g = new GameObject("Area Water - Poseidon Preview");
                    PoseidonAreaWaterPreview waterComponent = g.AddComponent<PoseidonAreaWaterPreview>();
                    Selection.activeObject = waterComponent;

                    if (GEditorCommon.SnapObjectToSceneViewLook(g))
                    {
                        //slightly offset water body up to avoid zfight with terrain surface
                        g.transform.position += Vector3.up;
                    }
                }
                GUI.enabled = false;
                GEditorCommon.ClickableCard(PageGUI.TILEABLE_WATER_IMAGE, "Tileable Water", "Repeat a water tile several times to cover a large area, great for ocean and open water.", "Available in Poseidon");
                GEditorCommon.ClickableCard(PageGUI.RIVER_WATER_IMAGE, "River Water", "Define water path with multiple splines, used for streams and rivers.", "Available in Poseidon");
                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();

                if (EditorGUILayout.LinkButton("Learn more about water bodies in Poseidon →"))
                {
                    GNetUtils.TrackClick("learn_more_water_bodies", GUILocation.Wizard);
                    Application.OpenURL("https://docs.pinwheelstud.io/poseidon/2/docs/create-water-body.html");
                }
            }
            else
            {
                EditorGUILayout.LabelField($"Poseidon {GetPoseidonVersionString()} provides multiple water body types. Choose one based on your use case:", GStyles.P1);

                EditorGUILayout.BeginHorizontal();
                if (GEditorCommon.ClickableCard(PageGUI.TILEABLE_WATER_IMAGE, "Tileable Water", "Repeat a water tile several times to cover a large area, great for ocean and open water."))
                {
                    GameObject g = AddTileableWater();
                    Selection.activeGameObject = g;
                    if (GEditorCommon.SnapObjectToSceneViewLook(g))
                    {
                        //slightly offset water body up to avoid zfight with terrain surface
                        g.transform.position += Vector3.up;
                    }
                }
                if (GEditorCommon.ClickableCard(PageGUI.AREA_WATER_IMAGE, "Area Water", "Use anchors to define water shape, suitable for medium to small water such as ponds, lakes, puddles."))
                {
                    GameObject g = AddAreaWater();
                    Selection.activeGameObject = g;
                    if (GEditorCommon.SnapObjectToSceneViewLook(g))
                    {
                        //slightly offset water body up to avoid zfight with terrain surface
                        g.transform.position += Vector3.up;
                    }
                }
                if (GEditorCommon.ClickableCard(PageGUI.RIVER_WATER_IMAGE, "River Water", "Define water path with multiple splines, used for streams and rivers."))
                {
                    GameObject g = AddRiverWater();
                    Selection.activeGameObject = g;
                    if (GEditorCommon.SnapObjectToSceneViewLook(g))
                    {
                        //slightly offset water body up to avoid zfight with terrain surface
                        g.transform.position += Vector3.up;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (EditorGUILayout.LinkButton("Learn more about water bodies →"))
                {
                    Application.OpenURL("https://docs.pinwheelstud.io/poseidon/2/docs/create-water-body.html");
                }
            }
        }

        private void DrawPreviewWaterList()
        {
            PoseidonAreaWaterPreview[] poseidonWatersPreview = Object.FindObjectsByType<PoseidonAreaWaterPreview>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (poseidonWatersPreview.Length == 0)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Preview waters in scene: {poseidonWatersPreview.Length}", GStyles.H3);
            if (IsPoseidonInstalled())
            {
                for (int i = 0; i < poseidonWatersPreview.Length; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(poseidonWatersPreview[i], typeof(PoseidonAreaWaterPreview), true);
                    if (GUILayout.Button("Convert to Poseidon Water", EditorStyles.miniButton, GUILayout.Width(200)))
                    {
                        GameObject poseidonWaterObject = ConvertToPoseidonWater(poseidonWatersPreview[i]);
                        if (poseidonWaterObject != null)
                        {
                            poseidonWaterObject.transform.parent = poseidonWatersPreview[i].transform.parent;
                            poseidonWaterObject.transform.localPosition = poseidonWatersPreview[i].transform.localPosition;
                            poseidonWaterObject.transform.localRotation = poseidonWatersPreview[i].transform.localRotation;
                            poseidonWaterObject.transform.localScale = poseidonWatersPreview[i].transform.localScale;

                            poseidonWatersPreview[i].gameObject.SetActive(false);
                            Selection.activeGameObject = poseidonWaterObject;
                            SceneView.lastActiveSceneView?.FrameSelected();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                for (int i = 0; i < poseidonWatersPreview.Length; ++i)
                {
                    EditorGUILayout.ObjectField(poseidonWatersPreview[i], typeof(PoseidonAreaWaterPreview), true);
                }
            }
        }

        private void DrawPoseidonWaterList()
        {
            if (IsPoseidonInstalled())
            {
                EditorGUILayout.Space();
                GameObject[] poseidonWaterObjects = GetPoseidonGameObjectsInScene();
                if (poseidonWaterObjects.Length > 0)
                {
                    EditorGUILayout.LabelField($"Poseidon waters in scene: {poseidonWaterObjects.Length}", GStyles.H3);
                    for (int i = 0; i < poseidonWaterObjects.Length; ++i)
                    {
                        EditorGUILayout.ObjectField(poseidonWaterObjects[i], typeof(GameObject), true);
                    }
                }
            }
        }
    }
}
#endif
