#if CF_GOOGLE_DRIVE

using System;
using System.Collections.Generic;
using System.IO;
using cfUnityEngine.Editor;
using cfUnityEngine.Util;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.GoogleDrive
{
    [Util.FilePath("Assets/GoogleDrive", "MirrorSetting")]
    public partial class GDriveMirrorSetting : EditorSetting<GDriveMirrorSetting>
    {
        [AssetPath]
        [SerializeField] private string _serviceAccountCredentialJsonPath;

        private string _serviceAccountCredentialJson = string.Empty;
        public string serviceAccountCredentialJson
        {
            get
            {
                if (!string.IsNullOrEmpty(_serviceAccountCredentialJson))
                {
                    return _serviceAccountCredentialJson;
                }

                var assetPath = _serviceAccountCredentialJsonPath;
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/{assetPath}");
                if (asset == null)
                {
                    Debug.LogError($"[GDriveMirrorSetting.serviceAccountCredentialJson] Asset is not a TextAsset: {assetPath}");
                    return string.Empty;
                }
                _serviceAccountCredentialJson = asset.text;
                return _serviceAccountCredentialJson;
            }
        }

        public MirrorItem[] items;

        private Dictionary<string, MirrorItem> _mirrorMap = new();
        public bool refreshOnEnterPlayMode = false;
        public Dictionary<string, MirrorItem> mirrorMap => _mirrorMap;
        
        [MethodButton]
        private void Refresh()
        {
            Debug.Log("[GDriveMirrorSetting.Refresh] refresh started");
            GDriveMirror.instance.RefreshWithProgressBar().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[GDriveMirrorSetting.Refresh] refresh failed: {task.Exception}");
                }
                else
                {
                    Debug.Log("[GDriveMirrorSetting.Refresh] refresh succeed");
                }
            });
        }
        
        [MethodButton]
        private void ClearAllAndRefresh()
        {
            Debug.Log("[GDriveMirrorSetting.ClearAllAndRefresh] clear all and refresh started");
            GDriveMirror.instance.Refresh();
            //GDriveMirror.instance.ClearAllAndRefreshWithProgressBar().ContinueWith(task =>
            //{
            //    if (task.IsFaulted)
            //    {
            //        Debug.LogError($"[GDriveMirrorSetting.ClearAllAndRefresh] clear all and refresh failed: {task.Exception}");
            //    }
            //    else
            //    {
            //        Debug.Log("[GDriveMirrorSetting.ClearAllAndRefresh] clear all and refresh succeed");
            //    }
            //});
        }

        private void OnValidate()
        {
            if(_mirrorMap.Count != items.Length)
            {
                _mirrorMap.Clear();
                
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.googleDriveLink)) continue;
                    var googleFileId = GoogleDriveUtil.ExtractFileId(item.googleDriveLink);
                    if (string.IsNullOrEmpty(googleFileId))
                    {
                        Debug.LogError($"Invalid googleDriveLink: {item.googleDriveLink}");
                        continue;
                    }
                    
                    if(!_mirrorMap.TryAdd(googleFileId, item))
                    {
                        var existing = _mirrorMap[item.googleDriveLink];
                        Debug.LogError($"Duplicate googleDriveId ({item.googleDriveLink}) found for ({existing.assetFolderPath}) and ({item.assetFolderPath}), ");
                    }
                }
            }
        }
    }

    [Serializable]
    public class MirrorItem
    {
        public string assetFolderPath;
        [UrlLink]
        public string googleDriveLink;
        public string assetNameOverride;
    }
}

#endif
