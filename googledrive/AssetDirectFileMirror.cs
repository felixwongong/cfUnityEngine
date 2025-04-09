using System.Collections.Generic;
using System.IO;
using Google.Apis.Drive.v3;
using UnityEngine;
using File = Google.Apis.Drive.v3.Data.File;

namespace cfUnityEngine.GoogleDrive
{
    public class AssetDirectFileMirror: IFileMirrorHandler
    {
        public void RefreshFiles(FilesResource filesResource, IEnumerable<File> files)
        {
            var setting = GDriveMirrorSetting.GetSetting();
            if (setting == null) return;
            
            var mirrorMap = setting.mirrorMap;
            
            foreach (var file in files)
            {
                var id = file.Id;

                if (mirrorMap.TryGetValue(id, out var mirror))
                {
                    var assetPath = mirror.mirrorAssetLocation;
                    var absolutePath = Path.Combine(Application.dataPath, assetPath);
                    
                    var directoryInfo = Directory.CreateDirectory(absolutePath);
                    var fileName = file.Name;
                    var searchedFile = directoryInfo.GetFiles(fileName, SearchOption.TopDirectoryOnly);
                }
            }
        }
    }
}