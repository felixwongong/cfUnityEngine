using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UIElements;

public class CfFeatureSettingEditor : EditorWindow
{
    private static readonly NamedBuildTarget[] BUILD_TARGETS = new[]
    {
        NamedBuildTarget.Standalone,
        NamedBuildTarget.Android,
        NamedBuildTarget.iOS,
        NamedBuildTarget.WebGL, 
    };
    
    [MenuItem("Cf Tools/Feature Setting")]
    public static void ShowPanel()
    {
        CfFeatureSettingEditor wnd = GetWindow<CfFeatureSettingEditor>();
        wnd.titleContent = new GUIContent("CfFeatureSettingEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        PlayerSettings.GetScriptingDefineSymbols(BUILD_TARGETS[0]);
    }
}
