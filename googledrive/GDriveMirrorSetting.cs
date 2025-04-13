#if CF_GOOGLE_DRIVE

using System;
using System.Collections.Generic;
using cfEngine.Extension;
using cfUnityEngine.Editor;
using cfUnityEngine.Util;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.GoogleDrive
{
    [Util.FilePath("Assets/GoogleDrive", "MirrorSetting")]
    public class GDriveMirrorSetting : EditorSetting<GDriveMirrorSetting>
    {
        public TextAsset serviceAccountCredentialJson;
        public MirrorItem[] items;

        private Dictionary<string, MirrorItem> _mirrorMap = new();
        public Dictionary<string, MirrorItem> mirrorMap => _mirrorMap;
        
        [MethodButton]
        private void Refresh()
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
        }

        private void OnValidate()
        {
            if(_mirrorMap.Count != items.Length)
            {
                _mirrorMap.Clear();
                
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.googleDriveId)) continue;
                    
                    if(!_mirrorMap.TryAdd(item.googleDriveId, item))
                    {
                        Debug.LogError($"Duplicate googleDriveId {item.googleDriveId}");
                    }
                }
            }
        }
    }

    [Serializable]
    public class MirrorItem
    {
        public string assetFolderPath;
        public string googleDriveId;
        [ReadOnly]
        public string optionalLocalAssetName;
    }
}

#endif
