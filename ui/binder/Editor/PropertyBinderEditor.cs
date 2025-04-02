using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.Editor
{
    [CustomEditor(typeof(PropertyBoolBinder), editorForChildClasses:true)]
    public class PropertyBoolBinderEditor : FieldDrawerEditor
    {
        protected override FieldInfo GetField() => typeof(PropertyBoolBinder).BaseType?.GetField("_cachedValueMap", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }
    
    [CustomEditor(typeof(PropertyFloatBinder), editorForChildClasses:true)]
    public class PropertyFloatBinderEditor : FieldDrawerEditor
    {
        protected override FieldInfo GetField() => typeof(PropertyFloatBinder).BaseType?.GetField("_cachedValueMap", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }
    
    [CustomEditor(typeof(PropertyIntBinder), editorForChildClasses:true)]
    public class PropertyIntBinderEditor : FieldDrawerEditor
    {
        protected override FieldInfo GetField() => typeof(PropertyIntBinder).BaseType?.GetField("_cachedValueMap", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }
    
    [CustomEditor(typeof(PropertyObjectBinder), editorForChildClasses:true)]
    public class PropertyObjectBinderEditor : FieldDrawerEditor
    {
        protected override FieldInfo GetField() => typeof(PropertyObjectBinder).BaseType?.GetField("_cachedValueMap", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }
    
    public abstract class FieldDrawerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = false;
            var field = GetField();
            if (field != null)
            {
                CustomInspector.DrawField(string.Empty, field.GetValue(target));
            }
            GUI.enabled = true;
        }

        protected abstract FieldInfo GetField();
    }
}