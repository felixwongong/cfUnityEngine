#if CF_GOOGLE_DRIVE && UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using UnityEngine;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using SystemFile = System.IO.File;

namespace cfUnityEngine.GoogleDrive
{
    public class AssetDirectFileMirror : IFileMirrorHandler
    {
        public async IAsyncEnumerable<RefreshStatus> RefreshFilesAsync(FilesResource filesResource, IReadOnlyList<GoogleFile> googleFiles,  IReadOnlyDictionary<string, MirrorItem> settingMap)
        {
            for (var i = 0; i < googleFiles.Count; i++)
            {
                var googleFile = googleFiles[i];
                if (!settingMap.TryGetValue(googleFile.Id, out var mirror))
                    continue;

                var directory = CreateAssetDirectoryIfNotExists(mirror.assetFolderPath);
                var localFileName = GetLocalFileName(googleFile, mirror);
                var fullPath = Path.Combine(directory.FullName, localFileName);

                using var fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);

                if (googleFile.isGoogleMimeType())
                {
                    var mimeType = googleFile.getExportMimeType();
                    var request = filesResource.Export(googleFile.Id, mimeType);

                    var result = await request.DownloadAsync(fileStream);
                    LogDownloadProgress(result, googleFile);

                    if (result != null && result.Status == DownloadStatus.Completed)
                    {
                        yield return new RefreshStatus(googleFile, result, (float)i / googleFiles.Count);
                    }
                }
                else
                {
                    yield return new RefreshStatus(googleFile, null, (float)i / googleFiles.Count);
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

                var fileStream = new FileStream(Path.Combine(directory.FullName, localFileName), FileMode.OpenOrCreate,
                    FileAccess.Write);

                if (googleFile.isGoogleMimeType())
                {
                    var mimeType = googleFile.getExportMimeType();
                    var request = filesResource.Export(googleFile.Id, mimeType);
                    var status = request.DownloadWithStatus(fileStream);
                    LogDownloadProgress(status, googleFile);
                    fileStream.Dispose();
                }
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
                localAssetName = $"{googleFile.Name}.{googleFile.getExportExtensionType()}";
            }

            return localAssetName;
        }

        private void LogDownloadProgress(IDownloadProgress progress, GoogleFile googleFile)
        {
            switch (progress.Status)
            {
                case DownloadStatus.Completed:
                    Debug.Log(
                        $"[AssetDirectFileMirror.RefreshFiles] Download completed, google file: {googleFile.Name}");
                    break;
                case DownloadStatus.Failed:
                    Debug.LogError(
                        $"[AssetDirectFileMirror.RefreshFiles] Download failed, google file: {googleFile.Name}, status: {progress.Status}");
                    break;
                default:
                    Debug.LogWarning(
                        $"[AssetDirectFileMirror.RefreshFiles] Download status: {progress.Status}, google file: {googleFile.WritersCanShare}");
                    break;
            }
        }
    }
}

#endif