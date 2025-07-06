using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.Util.Editor
{
    [CustomPropertyDrawer(typeof(GridMap<>))]
    public class GridMapDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var dimensionsProp = property.FindPropertyRelative("_dimensions");
            var listProp = property.FindPropertyRelative("_list");
            
            EditorGUI.BeginChangeCheck();
            var newDimensions = EditorGUILayout.Vector3IntField("Dimensions", dimensionsProp.vector3IntValue);
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log($"GridMap dimensions changed from {dimensionsProp.vector3IntValue} to {newDimensions}");
                dimensionsProp.vector3IntValue = newDimensions;
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}