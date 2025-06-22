#if CF_GOOGLE_DRIVE && UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using cfUnityEngine.Util;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using UnityEngine;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using SystemFile = System.IO.File;

namespace cfUnityEngine.GoogleDrive
{
    public class AssetDirectFileMirror : IFileMirrorHandler
    {
        public async IAsyncEnumerable<RefreshStatus> RefreshFilesAsync(DriveService driveService, RefreshRequest request)
        {
            var googleFiles = request.googleFiles;
            var getSetting = request.getSetting;
            var fileResource = driveService.Files;
            var changeHandler = request.changeHandler;
            for (var i = 0; i < googleFiles.Count; i++)
            {
                var googleFile = googleFiles[i];
                var getFileSetting = getSetting(googleFile);
                if (getFileSetting.TryGetError(out var error))
                {
                    Debug.LogError(error);
                    continue;
                }
                
                if (!getFileSetting.TryGetValue(out var optionalSetting) || !optionalSetting.TryGetValue(out var setting))
                    continue;
                
                if (!GoogleDriveUtil.MimeFileHandlers.TryGetValue(googleFile.MimeType, out var handler))
                {
                    Debug.Log($"[AssetDirectFileMirror.RefreshFilesAsync] No handler found for mime type: {googleFile.MimeType}");
                    continue;
                }   
                
                var directory = DirectoryUtil.CreateAssetDirectoryIfNotExists(setting.assetFolderPath);
                var localFileName = GetLocalFileName(googleFile, setting);
                
                var result = await handler.DownloadAsync(
                    fileResource,
                    new FileHandler.DownloadRequest
                    {
                        googleFileId = googleFile.Id,
                        rootDirectoryInfo = directory,
                        localName = localFileName,
                        changeHandler = changeHandler
                    }
                );
                
                LogDownloadProgress(result, googleFile);
                if (result != null && result.Status == DownloadStatus.Completed)
                {
                    yield return new RefreshStatus(googleFile, result, (float)i / googleFiles.Count);
                }
                OnDownloadEnd(result, googleFile, setting);
            }
        }

        public void RefreshFiles(DriveService driveService, in RefreshRequest request)
        {
            var filesResource = driveService.Files;
            var googleFiles = request.googleFiles;
            var getSetting = request.getSetting;
            var changeHandler = request.changeHandler;
            foreach (var googleFile in googleFiles)
            {
                var getFileSetting = getSetting(googleFile);
                if (getFileSetting.TryGetError(out var err))
                {
                    Debug.LogError(err);
                    continue;
                }

                if (!getFileSetting.TryGetValue(out var optionalSetting) || !optionalSetting.TryGetValue(out var setting))
                    continue;

                if (!GoogleDriveUtil.MimeFileHandlers.TryGetValue(googleFile.MimeType, out var handler))
                {
                    Debug.Log($"[AssetDirectFileMirror.RefreshFiles] No handler found for mime type: {googleFile.MimeType}");
                    continue;
                }

                var directory = DirectoryUtil.CreateAssetDirectoryIfNotExists(setting.assetFolderPath);
                var localFileName = GetLocalFileName(googleFile, setting);
                
                var result = handler.DownloadWithStatus(
                    filesResource,
                    new FileHandler.DownloadRequest
                    {
                        googleFileId = googleFile.Id,
                        rootDirectoryInfo = directory,
                        localName = localFileName,
                        changeHandler = changeHandler
                    }
                );

                LogDownloadProgress(result, googleFile);
                OnDownloadEnd(result, googleFile, setting);
            }
        }

        private void OnDownloadEnd(IDownloadProgress progress, GoogleFile googleFile, SettingItem setting)
        {
            if (progress is { Status: DownloadStatus.Completed })
            {
                setting.googleFileName = googleFile.Name;
            }
        }

        public void ClearAssetDirectories(IEnumerable<string> assetFolderPaths)
        {
            foreach (var assetFolderPath in assetFolderPaths)
            {
                var absolutePath = Path.Combine(Application.dataPath, assetFolderPath);
                var directoryInfo = new DirectoryInfo(absolutePath);
                if (!directoryInfo.Exists)
                    continue;
                
                directoryInfo.Delete(true);
            }
        }

        private string GetLocalFileName(GoogleFile googleFile, SettingItem setting)
        {
            var localAssetName = setting.assetNameOverride;
            if (string.IsNullOrWhiteSpace(localAssetName))
            {
                localAssetName = $"{googleFile.Name}";
            }

            return localAssetName;
        }

        private void LogDownloadProgress(IDownloadProgress progress, GoogleFile googleFile)
        {
            switch (progress.Status)
            {
                case DownloadStatus.Completed:
                    Debug.Log($"[AssetDirectFileMirror.RefreshFiles] Download completed, google file: {googleFile.Name}");
                    break;
                case DownloadStatus.Failed:
                    Debug.LogError($"[AssetDirectFileMirror.RefreshFiles] Download failed, google file: {googleFile.Name}, status: {progress.Status}\n Error: {progress.Exception?.Message}");
                    break;
                default:
                    Debug.LogWarning($"[AssetDirectFileMirror.RefreshFiles] Download status: {progress.Status}, google file: {googleFile.WritersCanShare}");
                    break;
            }
        }
    }
}

#endif