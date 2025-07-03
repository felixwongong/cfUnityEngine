#if CF_GOOGLE_DRIVE

using System;
using System.Collections.Generic;
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

        public string serviceAccountCredentialJson
        {
            get
            {
                var assetPath = _serviceAccountCredentialJsonPath;
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/{assetPath}");
                if (asset == null)
                {
                    Debug.LogError($"[GDriveMirrorSetting.serviceAccountCredentialJson] Asset is not a TextAsset: {assetPath}");
                    return string.Empty;
                }
                return asset.text;
            }
        }
        [ReadOnly]
        public string changeChecksumToken = string.Empty;

        public SettingItem[] items;

        private Dictionary<string, SettingItem> _settingMap = new();
        public bool refreshOnEnterPlayMode = false;
        public Dictionary<string, SettingItem> settingMap => _settingMap;

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
                    AssetDatabase.Refresh();
                    Debug.Log("[GDriveMirrorSetting.Refresh] refresh succeed");
                }
            });
        }

        [MethodButton]
        private void ForceRefreshAll()
        {
            Debug.Log("[GDriveMirrorSetting.ClearAllAndRefresh] clear all and refresh started");
            GDriveMirror.instance.ClearAllAndRefreshWithProgressBar().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"[GDriveMirrorSetting.ClearAllAndRefresh] clear all and refresh failed: {task.Exception}");
                }
                else
                {
                    AssetDatabase.Refresh();
                    Debug.Log("[GDriveMirrorSetting.ClearAllAndRefresh] clear all and refresh succeed");
                }
            });
        }

        private void OnValidate()
        {
            _settingMap.Clear();
            
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.googleDriveLink)) continue;
                var getUrlInfo = GoogleDriveUtil.ParseUrl(item.googleDriveLink);
                if (getUrlInfo.TryGetError(out var error))
                {
                    Debug.LogError(error);
                    continue;
                }

                if (!getUrlInfo.TryGetValue(out var urlInfo))
                    continue;
                
                if(!_settingMap.TryAdd(urlInfo.fileId, item))
                {
                    var existing = _settingMap[item.googleDriveLink];
                    Debug.LogError($"Duplicate googleDriveId ({item.googleDriveLink}) found for ({existing.assetFolderPath}) and ({item.assetFolderPath}), ");
                }
            }
        }
    }

    [Serializable]
    public class SettingItem
    {
        public string assetNameOverride;
        public string assetFolderPath;
        [DriveUrlLink]
        public string googleDriveLink;
        [ReadOnly]
        public string googleFileName;
        [SerializeField,HideInInspector]
        public List<string> googleFiles;

        public void ClearCache()
        {
            googleFileName = string.Empty;
        }
    }
}

#endif
