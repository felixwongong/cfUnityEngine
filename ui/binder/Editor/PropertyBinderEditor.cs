using System.Reflection;
using UnityEditor;

namespace cfUnityEngine.Editor
{
    [CustomEditor(typeof(PropertyBoolBinder), editorForChildClasses:true)]
    public class PropertyBinderEditor : UnityEditor.Editor
    {
        private bool expanded = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var fieldInfo = typeof(PropertyBoolBinder).BaseType?.GetField("__cachedValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
            if (fieldInfo != null)
            {
                expanded = EditorGUILayout.Foldout(expanded, "Cached Property");
                if (expanded)
                {
                    CustomInspector.DrawField(typeof(bool), "Value", fieldInfo.GetValue(target));
                }
            }
        }
    }
}