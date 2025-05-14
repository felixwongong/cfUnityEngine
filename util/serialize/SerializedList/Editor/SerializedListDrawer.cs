using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace cfUnityEngine.Util.Editor
{
    [CustomPropertyDrawer(typeof(SerializedList<>), false)]
    public class SerializedListDrawer: PropertyDrawer
    {
        private ReorderableList list;
        private SerializedProperty _listProperty;
        private SerializedProperty _property;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            _property = property;
            _listProperty = property.FindPropertyRelative("_serializedList");
            
            list = new ReorderableList(_listProperty.serializedObject, _listProperty, true, true, true, true);

            list.drawHeaderCallback += DrawHeader;
            list.drawElementCallback += DrawListItems;
            list.elementHeightCallback = GetElementPropertyHeight;
            list.onAddCallback += OnListAddCallback;
            
            list.DoLayoutList();
            property.serializedObject.ApplyModifiedProperties();
        }

        private float GetElementPropertyHeight(int index)
        {
            var element = _listProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        private void OnListAddCallback(ReorderableList reorderableList)
        {
            _listProperty.arraySize++;
            var newElement = _listProperty.GetArrayElementAtIndex(_listProperty.arraySize - 1);
            var listObjectProperty = newElement.FindPropertyRelative("listObject");
            var typeAssemblyNameProperty = newElement.FindPropertyRelative("typeAssemblyName");
            listObjectProperty.managedReferenceValue = null;
            typeAssemblyNameProperty.stringValue = string.Empty;
        }

        private void DrawHeader(Rect rect)
        {
            if (_property == null)
                return;
            
            EditorGUI.LabelField(rect, _property.displayName);
        }
        
        private void DrawListItems(Rect rect, int index, bool isactive, bool isfocused)
        {
            var elementProperty = list.serializedProperty.GetArrayElementAtIndex(index);
            var elementRect = new Rect(rect.x + 10f, rect.y, rect.width, EditorGUI.GetPropertyHeight(elementProperty, true) + EditorGUIUtility.standardVerticalSpacing);
            EditorGUI.PropertyField(elementRect, elementProperty);
        }
    }
}