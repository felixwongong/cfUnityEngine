using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace cfUnityEngine.Util.Editor
{
    [CustomPropertyDrawer(typeof(SerializedList<>), false)]
    public class SerializedListDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var serializedList = property.FindPropertyRelative("_serializedList");
            EditorGUILayout.PropertyField(serializedList, new GUIContent(property.displayName));
        }
    }
}