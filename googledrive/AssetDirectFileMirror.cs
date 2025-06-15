#if CF_GOOGLE_DRIVE && UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using UnityEngine;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using SystemFile = System.IO.File;
using MimeTypeStr = System.String;

namespace cfUnityEngine.GoogleDrive
{
    public class AssetDirectFileMirror : IFileMirrorHandler
    {
        private static Dictionary<MimeTypeStr, XlsxFileHandler> _fileHandlers = new()
        {
            { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", new XlsxFileHandler() },
        };

        public async IAsyncEnumerable<RefreshStatus> RefreshFilesAsync(FilesResource filesResource, IReadOnlyList<GoogleFile> googleFiles,  IReadOnlyDictionary<string, MirrorItem> settingMap)
        {
            for (var i = 0; i < googleFiles.Count; i++)
            {
                var googleFile = googleFiles[i];
                if (!settingMap.TryGetValue(googleFile.Id, out var mirror))
                    continue;

                var directory = CreateAssetDirectoryIfNotExists(mirror.assetFolderPath);
                var localFileName = GetLocalFileName(googleFile, mirror);

                await using var fileStream = new FileStream(Path.Combine(directory.FullName, localFileName), FileMode.OpenOrCreate, FileAccess.Write);

                if (!_fileHandlers.TryGetValue(googleFile.MimeType, out var handler))
                {
                    Debug.Log($"[AssetDirectFileMirror.RefreshFilesAsync] No handler found for mime type: {googleFile.MimeType}");
                    continue;
                }   
                
                var result = await handler.DownloadAsync(
                    filesResource,
                    fileStream,
                    new FileHandler.DownloadRequest { fileId = googleFile.Id }
                );
                
                LogDownloadProgress(result, googleFile);
                if (result != null && result.Status == DownloadStatus.Completed)
                {
                    yield return new RefreshStatus(googleFile, result, (float)i / googleFiles.Count);
                }
            }
        }

        public void RefreshFiles(FilesResource filesResource, IEnumerable<GoogleFile> googleFiles, IReadOnlyDictionary<string, MirrorItem> settingMap)
        {
            foreach (var googleFile in googleFiles)
            {
                if (!settingMap.TryGetValue(googleFile.Id, out var mirror))
                    continue;

                var directory = CreateAssetDirectoryIfNotExists(mirror.assetFolderPath);
                var localFileName = GetLocalFileName(googleFile, mirror);
                
                using var fileStream = new FileStream(Path.Combine(directory.FullName, localFileName), FileMode.OpenOrCreate, FileAccess.Write);
                
                if (!_fileHandlers.TryGetValue(googleFile.MimeType, out var handler))
                {
                    Debug.Log($"[AssetDirectFileMirror.RefreshFiles] No handler found for mime type: {googleFile.MimeType}");
                    continue;
                }   

                var result = handler.DownloadWithStatus(
                    filesResource,
                    fileStream,
                    new FileHandler.DownloadRequest { fileId = googleFile.Id }
                );

                LogDownloadProgress(result, googleFile);
            }
        }

        private DirectoryInfo CreateAssetDirectoryIfNotExists(string assetFolderPath)
        {
            var absolutePath = Path.Combine(Application.dataPath, assetFolderPath);
            var directoryInfo = new DirectoryInfo(absolutePath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            return directoryInfo;
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

        private string GetLocalFileName(GoogleFile googleFile, MirrorItem mirror)
        {
            var localAssetName = mirror.assetNameOverride;
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