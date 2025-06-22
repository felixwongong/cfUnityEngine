using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.Editor
{
    [CustomPropertyDrawer(typeof(UrlLinkAttribute), true)]
    public class UrlLinkAttributeDrawer: PropertyDrawer
    {
        private const float buttonWidth = 100f;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var textFieldRect = new Rect(position.x, position.y, position.width - buttonWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(textFieldRect, property);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            
            if (GUI.Button(new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, EditorGUIUtility.singleLineHeight), "Open Link"))
            {
                if (!string.IsNullOrEmpty(property.stringValue))
                {
                    Application.OpenURL(property.stringValue);
                }
                else
                {
                    Debug.LogWarning("UrlLinkAttributeDrawer: Property value is empty, cannot open link.");
                }
            }
        }
    }
}