using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.GoogleDrive
{
    [InitializeOnLoad]
    public partial class GDriveMirror
    {
        public static GDriveMirror instance { get; }

        static GDriveMirror()
        {
            instance = new GDriveMirror(new AssetDirectFileMirror(), new UnityLogger());
        }

        public async Task RefreshWithProgressBar()
        {
            EditorUtility.DisplayProgressBar("GDriveMirror", "Refreshing Google Files...", 0f);
            try
            {
                await foreach (var status in RefreshAsync())
                {
                    EditorUtility.DisplayProgressBar("GDriveMirror", $"Google File refreshed, file: {status.file.Name}, status: {status.status.Status.ToString()}", status.progress);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            EditorUtility.ClearProgressBar();
        }

        public async Task ClearAllAndRefreshWithProgressBar()
        {
            EditorUtility.DisplayProgressBar("GDriveMirror", "Refreshing Google Files...", 0f);
            try
            {
                await foreach (var status in ClearAllAndRefreshAsync())
                {
                    EditorUtility.DisplayProgressBar("GDriveMirror", $"Google File refreshed, file: {status.file.Name}, status: {status.status.Status.ToString()}", status.progress);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}