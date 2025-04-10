using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using UnityEngine;
using File = Google.Apis.Drive.v3.Data.File;

namespace cfUnityEngine.GoogleDrive
{
    public class AssetDirectFileMirror: IFileMirrorHandler
    {
        public Task RefreshFiles(FilesResource filesResource, IEnumerable<File> googleFiles)
        {
            var setting = GDriveMirrorSetting.GetSetting();
            if (setting == null) return Task.FromException(new InvalidOperationException("[AssetDirectFileMirror.RefreshFiles] GDriveMirrorSetting is null"));
            
            var mirrorMap = setting.mirrorMap;

            List<Task> downloadTasks = new List<Task>();
            foreach (var googleFile in googleFiles)
            {
                var id = googleFile.Id;

                if (mirrorMap.TryGetValue(id, out var mirror))
                {
                    var assetFolderPath = mirror.assetFolderPath;
                    var absolutePath = Path.Combine(Application.dataPath, assetFolderPath);

                    if (!Directory.Exists(absolutePath))
                        Directory.CreateDirectory(absolutePath);
                    
                    var fileName = googleFile.Name;
                    var fileStream = new FileStream(Path.Combine(absolutePath, $"{fileName}.{googleFile.getExportExtensionType()}"), FileMode.OpenOrCreate, FileAccess.Write) ;
                    
                    if (googleFile.isGoogleMimeType())
                    {
                        var mimeType = googleFile.getExportMimeType();
                        var request = filesResource.Export(googleFile.Id, mimeType);
                        var downloadTask = request.DownloadAsync(fileStream).ContinueWith(task =>
                        {
                            fileStream.Dispose();
                        });
                        downloadTasks.Add(downloadTask);
                    }
                }
            }

            return Task.WhenAll(downloadTasks);
        }
    }
}