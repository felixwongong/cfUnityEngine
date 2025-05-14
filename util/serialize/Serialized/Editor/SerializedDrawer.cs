using System;
using System.Collections.Generic;
using System.Linq;
using cfEngine.Util;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace cfUnityEngine.Util.Editor
{
    [CustomPropertyDrawer(typeof(Serialized<>))]
    public class SerializedDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var typeAssemblyNameProperty = property.FindPropertyRelative("typeAssemblyName");
            var listObjectType = Type.GetType(typeAssemblyNameProperty.stringValue);
            var listObjectProperty = property.FindPropertyRelative("listObject");
            var idProperty = listObjectProperty.FindPropertyRelative("id");
            var foldoutName = idProperty != null ? idProperty.stringValue : listObjectProperty.name;
            
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, 50, EditorGUIUtility.singleLineHeight), property.isExpanded, foldoutName, true);

            if (property.isExpanded)
            {
                var listObjectTypes = GetAllPossibleListObjectTypes(listObjectType);
                var selectedIndex = Array.IndexOf(listObjectTypes, Type.GetType(typeAssemblyNameProperty.stringValue));
                
                int lineHeight = 1;
                var previousSelectedIndex = selectedIndex;
                selectedIndex = EditorGUI.Popup(new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lineHeight++, position.width, EditorGUIUtility.singleLineHeight), selectedIndex, listObjectTypes.Select(type => type.Name).ToArray());
                if (selectedIndex != previousSelectedIndex)
                {
                    typeAssemblyNameProperty.stringValue = listObjectTypes[selectedIndex].AssemblyQualifiedName;
                    typeAssemblyNameProperty.serializedObject.ApplyModifiedProperties();
                }
                
                if (!listObjectProperty.Next(true))
                    return;
                
                var depth = listObjectProperty.depth;
                do
                {
                    EditorGUI.PropertyField( new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lineHeight++, position.width, EditorGUIUtility.singleLineHeight), listObjectProperty);
                } while (listObjectProperty.Next(false) && listObjectProperty.depth == depth);

                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLine = 1;
            if (property.isExpanded)
            {
                totalLine += 1; //typeAssemblyName
                var listObjectProperty = property.FindPropertyRelative("listObject");
                if (!listObjectProperty.Next(true))
                    return totalLine * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * (totalLine - 1);
                
                var depth = listObjectProperty.depth;
                do
                {
                    totalLine += 1;
                } while (listObjectProperty.Next(false) && listObjectProperty.depth == depth);
            }
            return totalLine * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * (totalLine - 1);
        }

        private static Dictionary<Type, Type[]> _listPossibleTypesCache = new();
        private Type[] GetAllPossibleListObjectTypes(Type type)
        {
            if(!_listPossibleTypesCache.TryGetValue(type, out var types))
            {
                var baseType = type;
                while (baseType?.BaseType != null && baseType.BaseType != typeof(object) && baseType.BaseType != typeof(Object))
                {
                    baseType = baseType?.BaseType;
                }
                
                var typeList = new List<Type>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    typeList.AddRange(TypeExtension.FindDerivedTypes(assembly, baseType).Where(t => !t.IsAbstract && !t.IsInterface));
                }

                types = typeList.ToArray();
                _listPossibleTypesCache[type] = types;
            }
            
            return _listPossibleTypesCache[type];
        }
    }
}