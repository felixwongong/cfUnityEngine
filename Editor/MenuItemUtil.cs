using System.Linq;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.Editor
{
    public static class MenuItemUtil
    {
        public static bool TryOpenMenuWindow<T>(string menuItemName, string windowTypeName, out T editorWindow) where T : EditorWindow
        {
            editorWindow = null;
            if (!EditorApplication.ExecuteMenuItem(menuItemName))
            {
                return false;
            }
            var allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            editorWindow = allWindows.FirstOrDefault(w => w.GetType().Name.Equals(windowTypeName)) as T;
            return editorWindow != null;
        }
    }
}