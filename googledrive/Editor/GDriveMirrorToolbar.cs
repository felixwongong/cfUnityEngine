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
    class GoogleDriveTool : EditorTool
    {
       
    }
   
    [CustomEditor(typeof(GoogleDriveTool))]
    class GoogleDriveToolEditor : UnityEditor.Editor, ICreateToolbar
    {
        public IEnumerable<string> toolbarElements
        {
            get
            {
                yield return "GDriveMirror/Refresh";
            }
        }
    }
    
    [EditorToolbarElement("GDriveMirror/Refresh")]
    public class GDriveMirrorToolbar : EditorToolbarButton
    {
        public GDriveMirrorToolbar()
        {
            iconImage = Background.FromTexture2D((Texture2D)EditorGUIUtility.IconContent("Refresh").image);
            tooltip = "Refresh GDrive files";
            clicked += () =>
            {
                text = null;
                SetEnabled(false);
                GDriveMirror.instance.RefreshAsync()
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
}
#endif