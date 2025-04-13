#if CF_GOOGLE_DRIVE && UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using UnityEngine;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using SystemFile = System.IO.File;

namespace cfUnityEngine.GoogleDrive
{
    public class AssetDirectFileMirror: IFileMirrorHandler
    {
        public Task RefreshFilesAsync(FilesResource filesResource, IEnumerable<GoogleFile> googleFiles)
        {
            var setting = GDriveMirrorSetting.GetSetting();
            if (setting == null) return Task.FromException(new InvalidOperationException("[AssetDirectFileMirror.RefreshFilesAsync] GDriveMirrorSetting is null"));
            
            var mirrorMap = setting.mirrorMap;

            List<Task> downloadTasks = new List<Task>();
            foreach (var googleFile in googleFiles)
            {
                if (!mirrorMap.TryGetValue(googleFile.Id, out var mirror))
                    continue;
                
                var assetFolderPath = mirror.assetFolderPath;
                var absolutePath = Path.Combine(Application.dataPath, assetFolderPath);

                if (!Directory.Exists(absolutePath))
                    Directory.CreateDirectory(absolutePath);
                
                var googleFileName = googleFile.Name;
                var googleFileWithExtension = $"{googleFileName}.{googleFile.getExportExtensionType()}";
                var localAssetName = mirror.optionalLocalAssetName;
                if (string.IsNullOrWhiteSpace(localAssetName))
                {
                    localAssetName = googleFileWithExtension;
                }
                
                var fileStream = new FileStream(Path.Combine(absolutePath, localAssetName), FileMode.OpenOrCreate, FileAccess.Write) ;
                
                if (googleFile.isGoogleMimeType())
                {
                    var mimeType = googleFile.getExportMimeType();
                    var request = filesResource.Export(googleFile.Id, mimeType);
                    var downloadTask = request.DownloadAsync(fileStream).ContinueWith(task =>
                    {
                        fileStream.Dispose();
                        if (task.IsCompletedSuccessfully)
                        {
                            mirror.optionalLocalAssetName = localAssetName;
                        }
                    });
                    downloadTasks.Add(downloadTask);
                }
            }

            return Task.WhenAll(downloadTasks);
        }

        public void RefreshFiles(FilesResource filesResource, IEnumerable<GoogleFile> googleFiles)
        {
            var setting = GDriveMirrorSetting.GetSetting();
            if (setting == null)
            {
                Debug.LogException(new InvalidOperationException("[AssetDirectFileMirror.RefreshFiles] GDriveMirrorSetting is null"));
                return;
            }
            
            var mirrorMap = setting.mirrorMap;
            
            
        }
    }
}

#endif
