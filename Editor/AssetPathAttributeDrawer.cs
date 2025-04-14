using System.IO;
using UnityEditor;
using UnityEngine;

namespace cfUnityEngine.Editor
{
    [CustomPropertyDrawer(typeof(AssetPathAttribute))]
    public class AssetPathAttributeDrawer : PropertyDrawer
    {
        private static readonly string ApplicationDataPathForReplace = $"{Application.dataPath}/";
        
        /*public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var rootContainer = new VisualElement();
            var propertyContainer = new VisualElement();
            var field = new TextField() { value = property.stringValue };
            var nameField = new Label(property.displayName);
            var buttonRoot = new VisualElement();
            var searchButton = new Button();
            var folderButton = new Button();

            #region Styling & Layout

            rootContainer.style.alignItems = Align.Stretch;
            rootContainer.style.flexDirection = FlexDirection.Row;
            rootContainer.style.marginLeft = 5;
            rootContainer.style.marginTop = 3;
            propertyContainer.style.flexDirection = FlexDirection.Row;
            propertyContainer.style.justifyContent = Justify.SpaceBetween;
            propertyContainer.style.flexBasis = Length.Percent(40);
            field.style.flexBasis = Length.Percent(60);

            buttonRoot.style.flexDirection = FlexDirection.Row;
            searchButton.style.backgroundImage = new StyleBackground((Texture2D)EditorGUIUtility.IconContent("_Popup").image);
            searchButton.style.width = searchButton.style.height = 20;
            folderButton.style.backgroundImage = new StyleBackground((Texture2D)EditorGUIUtility.IconContent("d_Collab.FolderMoved").image);
            folderButton.style.width = folderButton.style.height = 20;

            propertyContainer.Add(nameField);
            propertyContainer.Add(buttonRoot);
            
            buttonRoot.Add(folderButton);
            buttonRoot.Add(searchButton);
            
            rootContainer.Add(propertyContainer);
            rootContainer.Add(field);

            #endregion

            field.RegisterValueChangedCallback(cb =>
            {
                property.serializedObject.Update();
            });
            
            searchButton.clickable.clicked += () =>
            {
                var path = EditorUtility.OpenFolderPanel("Select Directory", "Assets", "");
                if (string.IsNullOrEmpty(path)) return;

                path = path.AsSpan(path.IndexOf("Assets", StringComparison.Ordinal)).ToString();
                property.stringValue = path;
                property.serializedObject.ApplyModifiedProperties();
                field.value = path;
                property.serializedObject.Update();
            };

            if (string.IsNullOrEmpty(property.stringValue) || !AssetDatabase.IsValidFolder(property.stringValue))
                buttonRoot.Remove(folderButton);
            else
            {
                folderButton.clickable.clicked += () =>
                {
                    EditorDirectoryUtil.FocusDirectory(property.stringValue);
                };
            }

            return rootContainer;
        }*/
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Reserve rects
            float labelWidth = EditorGUIUtility.labelWidth;
            float buttonWidth = 22f;

            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect fieldRect = new Rect(position.x + labelWidth, position.y,position.width - labelWidth - buttonWidth * 2, position.height);
            Rect folderButtonRect = new Rect(fieldRect.xMax, position.y, buttonWidth, position.height);
            Rect searchButtonRect = new Rect(folderButtonRect.xMax, position.y, buttonWidth, position.height);

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.LabelField(labelRect, label);
            string newValue = EditorGUI.DelayedTextField(fieldRect, GUIContent.none, property.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = newValue;
            }
            
            // Folder button
            if (!string.IsNullOrEmpty(property.stringValue) && AssetDatabase.IsValidFolder(property.stringValue))
            {
                if (GUI.Button(folderButtonRect, EditorGUIUtility.IconContent("Prefab Icon"), GUIStyle.none))
                {
                    EditorDirectoryUtil.FocusDirectory(property.stringValue); // Your custom util
                }
            }

            // Search button
            if (GUI.Button(searchButtonRect, EditorGUIUtility.IconContent("_Popup"), GUIStyle.none))
            {
                var path = EditorUtility.OpenFilePanel("Select File", "Assets", "");
                property.stringValue = path.Replace(ApplicationDataPathForReplace, string.Empty).Replace("\\", "/");
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
                GUIUtility.ExitGUI();
            }
            
            EditorGUI.EndProperty();
        }
    }
}