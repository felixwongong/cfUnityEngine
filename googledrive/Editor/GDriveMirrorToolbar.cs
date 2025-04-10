#if CF_GOOGLE_DRIVE
using System.Collections.Generic;
using cfEngine.Extension;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

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
// This is retrieving the built-in Visual Elements for pivot mode and pivot rotation to add them to your tool settings toolbar
                yield return "Tool Settings/Pivot Mode";
                yield return "Tool Settings/Pivot Rotation";
                
//This is adding your custom element
                yield return "GDriveMirror/Refresh";
            }
        }
    }
    [EditorToolbarElement("GDriveMirror/Refresh")]
    public class GDriveMirrorToolbar : EditorToolbarButton
    {
        public GDriveMirrorToolbar()
        {
            text = "Refresh";
            tooltip = "Refresh GDrive files";
            clicked += () =>
            {
                GDriveMirror.instance.RefreshAsync()
                    .ContinueWithSynchronized(result =>
                    {
                        if (result.IsFaulted)
                        {
                            Debug.LogError(result.Exception);
                        }
                        else
                        {
                            Debug.Log("Refresh completed");
                        }
                    });
            };
        }
    }
}
#endif