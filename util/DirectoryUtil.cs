using System.IO;
using UnityEngine;

namespace cfUnityEngine.Util
{
    public static class DirectoryUtil
    {
        public static DirectoryInfo CreateAssetDirectoryIfNotExists(string assetFolderPath)
        {
            var absolutePath = Path.Combine(Application.dataPath, assetFolderPath);
            var directoryInfo = new DirectoryInfo(absolutePath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            return directoryInfo;
        }
    }
}