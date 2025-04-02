using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.Editor
{
    [CustomEditor(typeof(NamespaceScope), true)]
    public class NamespaceScopeEditor : UnityEditor.Editor
    {
        private readonly FieldInfo cachedSourcesField = typeof(NamespaceScope).GetField("__cachedSources", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUI.enabled = false;
            
            var cachedSources = (Dictionary<string, (IPropertySource, bool)>)cachedSourcesField.GetValue(target);
            if (cachedSources == null)
            {
                return;
            }
            
            foreach (var (nsName, (source, isBinding)) in cachedSources)
            {
                var fieldName = isBinding ? nsName : $"{nsName} (Missing)";
                CustomInspector.DrawField(fieldName, source);
            }
            
            GUI.enabled = true;
        }
    }
}