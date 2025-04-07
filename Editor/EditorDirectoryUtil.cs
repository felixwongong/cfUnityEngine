using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace cfUnityEngine.Editor
{
    public class EditorDirectoryUtil
    {
        public static void FocusDirectory(string path)
        {
            Object folderMeta = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (folderMeta != null)
            {
                Selection.activeObject = folderMeta;
            }
            else
            {
                Debug.Log($"folder ({path}) does not exist");
            }
        }
    }
}
