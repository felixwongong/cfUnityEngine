#if CF_GOOGLE_DRIVE
using System.Collections.Generic;
using cfEngine.Extension;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace cfUnityEngine.GoogleDrive
{
    [EditorTool("Drive")]
    class GoogleDriveEditorTool : EditorTool
    {
       
    }
   
    [CustomEditor(typeof(GoogleDriveEditorTool))]
    class GoogleDriveToolEditor : UnityEditor.Editor, ICreateToolbar
    {
        public IEnumerable<string> toolbarElements
        {
            get
            {
                yield return "GDriveMirror/Refresh";
                yield return "GDriveMirror/Show Setting";
            }
        }
    }
    
    [EditorToolbarElement("GDriveMirror/Refresh")]
    public class GDriveMirrorToolbarButton_Refresh : EditorToolbarButton
    {
        public GDriveMirrorToolbarButton_Refresh()
        {
            icon = (Texture2D)EditorGUIUtility.IconContent("Refresh").image;
            tooltip = "Refresh GDrive files";
            clicked += () =>
            {
                text = null;
                SetEnabled(false);
                GDriveMirror.instance.RefreshWithProgressBar()
                    .ContinueWithSynchronized(result =>
                    {
                        SetEnabled(true);
                        if (result.IsFaulted)
                        {
                            Debug.LogError(result.Exception);
                        }
                        else
                        {
                            Debug.Log("Refresh completed");
                        }
                        
                        SetEnabled(true);
                    });
            };
        }
    }
    
    [EditorToolbarElement("GDriveMirror/Show Setting")]
    public class GDriveMirrorToolbarButton_ShowSetting : EditorToolbarButton
    {
        public GDriveMirrorToolbarButton_ShowSetting()
        {
            icon = (Texture2D)EditorGUIUtility.IconContent("SettingsIcon").image;
            tooltip = "Show Mirror Setting";
            clicked += () =>
            {
                Selection.activeObject = GDriveMirrorSetting.GetSetting();
            };
        }
    }
}
#endif