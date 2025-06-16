using cfUnityEngine.Editor;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.GoogleDrive
{
    [CustomPropertyDrawer(typeof(DriveUrlLinkAttribute), true)]
    public class DriveUrlLinkAttributeDrawer: UrlLinkAttributeDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
            var url = property.stringValue;
            var totalLine = 0;

            GUI.enabled = false;
            property.isExpanded = EditorGUI.Foldout(GetShiftedLineRect(position, totalLine++), property.isExpanded, "UrlInfo", true);
            if (property.isExpanded)
            {
                var getUrlInfo = GoogleDriveUtil.ParseUrl(url);
                if (getUrlInfo.TryGetError(out var error))
                {
                    EditorGUI.LabelField(GetShiftedLineRect(position, totalLine++), $"Error: {error.Message}");
                } else if (getUrlInfo.TryGetValue(out var urlInfo))
                {
                    EditorGUI.EnumPopup(GetShiftedLineRect(position, totalLine++), "File Type", urlInfo.fileType);
                    EditorGUI.TextField(GetShiftedLineRect(position, totalLine++), "File ID", urlInfo.fileId);
                }
            }
            GUI.enabled = true;
        }

        private Rect GetShiftedLineRect(Rect propertyRect, int totalLine)
        {
            return new Rect(propertyRect.x,
                propertyRect.y + (totalLine + 1) *
                (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
                propertyRect.width, EditorGUIUtility.singleLineHeight);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var totalLine = property.isExpanded ? 3 : 1;
            return base.GetPropertyHeight(property, label) + 
                   (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * totalLine;
        }
    }
}