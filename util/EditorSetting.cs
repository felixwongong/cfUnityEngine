using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace cfUnityEngine.Util
{
    [Conditional("UNITY_EDITOR"), AttributeUsage(AttributeTargets.Class)]
    public class FilePathAttribute : Attribute
    {
        public readonly string folderPath;
        public readonly string fileName;
        public FilePathAttribute(string folderPath, string fileName)
        {
            this.folderPath = folderPath;
            this.fileName = fileName;
        }
    }
    
    public abstract class EditorSetting<TSetting> : ScriptableObject where TSetting: EditorSetting<TSetting>
    {
        private static readonly (string folderPath, string fileName) filePath = GetFilePath();
        private static readonly string[] searchFolder = { filePath.folderPath };
        private static TSetting _instance;
        public static TSetting GetSetting()
        {
            if (_instance != null)
                return _instance;

            if (string.IsNullOrEmpty(filePath.folderPath) || string.IsNullOrEmpty(filePath.fileName))
                return null;
        
            var guids = AssetDatabase.FindAssets($"{filePath.fileName} t:ScriptableObject", searchFolder);
            if (guids.Length <= 0)
            {
                AssetDatabase.CreateAsset(CreateInstance<TSetting>(), $"{filePath.folderPath}/{filePath.fileName}.asset");
                guids = AssetDatabase.FindAssets($"{filePath.fileName} t:ScriptableObject", searchFolder);
                if (guids.Length == 0)
                {
                    return null;
                }
            }
        
            if (guids.Length > 1)
            {
                Debug.LogWarning($"[EditorSetting.GetSetting] more than 1 instance of {filePath.fileName}");
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError($"[EditorSetting.GetSetting] asset not found for guid {guids[0]}");
                return null;
            }

            _instance = AssetDatabase.LoadAssetAtPath<TSetting>(assetPath);
            return _instance;
        }

        private static (string folderPath, string fileName) GetFilePath()
        {
            var attributes = typeof(TSetting).GetCustomAttributes(typeof(FilePathAttribute), true);
            if (attributes.Length == 0 || attributes[0] is not FilePathAttribute filePath)
            {
                Debug.Log($"[EditorSetting.GetFilePath] Setting type {typeof(TSetting).Name} does not have [FilePath] attribute implemented");
                return (string.Empty, string.Empty);
            }

            return (filePath.folderPath, filePath.fileName);
        }
    }
}