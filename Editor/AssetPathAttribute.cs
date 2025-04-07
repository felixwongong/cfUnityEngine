using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace cfUnityEngine.Editor
{
    [Conditional("UNITY_EDITOR"), AttributeUsage(AttributeTargets.Field)]
    public class AssetPathAttribute : PropertyAttribute
    {
    }

    [CustomPropertyDrawer(typeof(AssetPathAttribute))]
    public class AssetPathAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
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
        }
    }
}