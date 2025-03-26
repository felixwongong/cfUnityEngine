using System.Linq;
using System.Reflection;
using cfUnityEngine.Editor;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.UI.UIToolkit.Editor
{
    [CustomEditor(typeof(UIToolkitRoot))]
    public class UIRootEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open Debugger"))
            {
                OpenUIDebugger();
            }
        }

        private const string DebuggerWindowTypeName = "UIElementsDebugger";

        public static void OpenUIDebugger()
        {
            // Try to find an existing instance
            var allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            EditorWindow debuggerWindow =
                allWindows.FirstOrDefault(w => w.GetType().Name.Equals(DebuggerWindowTypeName));

            if (debuggerWindow == null && !MenuItemUtil.TryOpenMenuWindow("Window/UI Toolkit/Debugger",
                    DebuggerWindowTypeName, out debuggerWindow))
            {
                Debug.LogError($"Failed to open {DebuggerWindowTypeName} from menu.");
                return;
            }

            if (!debuggerWindow.hasFocus) debuggerWindow.Focus();

            var debuggerType = debuggerWindow.GetType();
            var debuggerField = debuggerType.GetField("m_DebuggerContext", BindingFlags.NonPublic | BindingFlags.Instance);
            if (debuggerField == null)
            {
                Debug.LogError("Couldn't find m_Debugger field.");
                return;
            }

            var debuggerInstance = debuggerField.GetValue(debuggerWindow);
            if (debuggerInstance == null)
            {
                Debug.LogError("Debugger instance is null.");
                return;
            }

            var debuggerInstanceType = debuggerInstance.GetType();

            // Try selecting the first panel via SelectPanel(VisualElementPanel panel) or similar
            var panelsProperty =
                debuggerInstanceType.GetProperty("panels", BindingFlags.NonPublic | BindingFlags.Instance);
            if (panelsProperty == null)
            {
                Debug.LogError("Couldn't find 'panels' property.");
                return;
            }

            var panels = panelsProperty.GetValue(debuggerInstance) as System.Collections.IList;
            if (panels == null || panels.Count == 0)
            {
                Debug.LogError("No panels found.");
                return;
            }

            // Select the first panel
            var firstPanel = panels[0];

            var selectPanelMethod =
                debuggerInstanceType.GetMethod("SelectPanel", BindingFlags.NonPublic | BindingFlags.Instance);
            if (selectPanelMethod == null)
            {
                Debug.LogError("Couldn't find SelectPanel method.");
                return;
            }

            selectPanelMethod.Invoke(debuggerInstance, new[] { firstPanel });

            Debug.Log("Panel selected!");
        }
    }
}