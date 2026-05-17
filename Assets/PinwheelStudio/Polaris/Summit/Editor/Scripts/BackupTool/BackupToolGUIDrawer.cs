#if GRIFFIN && !GRIFFIN_EXCLUDE_SUMMIT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.Griffin.BackupTool
{
    [InitializeOnLoad]
    public static class BackupToolGUIDrawer
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            GBackupEditor.DrawCreateBackupCallback += OnDrawCreateBackupGUI;
            GBackupEditor.DrawAvailableBackupCallback += OnDrawAvailableBackupGUI;
        }

        private static void OnDrawCreateBackupGUI(GBackupEditor editor)
        {
            string label = "Create";
            string id = "createbackup";

            GEditorCommon.Foldout(label, false, id, () =>
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 100;

                editor.GroupID = GEditorCommon.ActiveTerrainGroupPopupWithAllOption("Group Id", editor.GroupID);
                GUI.enabled = !editor.UseAutoName;
                editor.BackupName = EditorGUILayout.TextField("Name", editor.BackupName);
                GUI.enabled = true;
                editor.UseAutoName = EditorGUILayout.Toggle("Auto Name", editor.UseAutoName);
                if (editor.UseAutoName)
                {
                    editor.BackupName = GBackupFile.GetBackupNameByTimeNow();
                }

                GUI.enabled = !string.IsNullOrEmpty(editor.BackupName);
                Rect r = EditorGUILayout.GetControlRect();
                if (GUI.Button(r, "Create"))
                {
                    EditorUtility.DisplayProgressBar("Backing Up", "Creating backup files...", 1);
                    GUndoCompatibleBuffer.Instance.RecordUndo();
                    GBackup.Create(editor.BackupName, editor.GroupID);
                    EditorUtility.ClearProgressBar();
                }
                GUI.enabled = true;

                EditorGUIUtility.labelWidth = labelWidth;
            });
        }

        private static void OnDrawAvailableBackupGUI(GBackupEditor editor)
        {
            string label = "Available Backups";
            string id = "availablebackups";

            GEditorCommon.Foldout(label, false, id, () =>
            {
                int entryCount = 0;
                for (int i = 0; i < editor.Backups.Count; ++i)
                {
                    if (!editor.Backups[i].StartsWith("~"))
                    {
                        entryCount += 1;
                        DrawBackupEntry(editor, editor.Backups[i]);
                    }
                }

                if (entryCount == 0)
                {
                    EditorGUILayout.LabelField("No Backup found!", GEditorCommon.WordWrapItalicLabel);
                }
            });
        }

        private static void DrawBackupEntry(GBackupEditor editor, string backupName)
        {
            Rect r = EditorGUILayout.GetControlRect();
            if (r.Contains(Event.current.mousePosition))
            {
                Color boxColor = EditorGUIUtility.isProSkin ? GEditorCommon.lightGrey : GEditorCommon.darkGrey;
                GEditorCommon.DrawOutlineBox(r, boxColor);
                if (Event.current != null && Event.current.type == EventType.MouseDown)
                {
                    if (Event.current.button == 0)
                    {
                        editor.ConfirmAndRestoreBackup(backupName);
                    }
                    else if (Event.current.button == 1)
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(
                            new GUIContent("Restore"),
                            false,
                            () => { editor.ConfirmAndRestoreBackup(backupName); });
                        menu.AddItem(
                            new GUIContent("Delete"),
                            false,
                            () => { editor.ConfirmAndDeleteBackup(backupName); });
                        menu.ShowAsContext();
                    }
                }
            }
            EditorGUI.LabelField(r, backupName);

            if (backupName.Equals(GUndoCompatibleBuffer.Instance.CurrentBackupName))
            {
                Rect dotRect = new Rect(r.x, r.y, r.height, r.height);
                GUI.Label(dotRect, GEditorCommon.dot);
            }
        }
    }
}
#endif