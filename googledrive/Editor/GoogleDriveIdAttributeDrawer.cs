using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.GoogleDrive
{
    [CustomPropertyDrawer(typeof(GoogleDriveIdAttribute), true)]
    public class GoogleDriveIdAttributeDrawer : PropertyDrawer
    {
        /*
        private string _driveLink = string.Empty; 
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (string.IsNullOrEmpty(_driveLink) && !string.IsNullOrEmpty(property.stringValue))
            {
                _driveLink = GoogleDriveUtil.FormLink(property.stringValue);
            }
            
            const float buttonWidth = 100f;
            var textFieldRect = new Rect(position.x, position.y, position.width - buttonWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            _driveLink = EditorGUI.TextField(textFieldRect, "Google Drive Link", _driveLink);
            if (EditorGUI.EndChangeCheck())
            {
                var fileId = GoogleDriveUtil.ExtractFileId(_driveLink);
                property.stringValue = fileId;
                _driveLink = GoogleDriveUtil.FormLink(fileId);
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            if(GUI.Button(new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, EditorGUIUtility.singleLineHeight), "Open Link") && !string.IsNullOrEmpty(_driveLink))
            {
                Application.OpenURL(_driveLink);
            }

            GUI.enabled = false;
            var rect = new Rect(position.x, position.y + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight, position.width, position.height);
            EditorGUI.PropertyField(rect, property);
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        */
    }
}