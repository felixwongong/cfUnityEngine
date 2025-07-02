using System;
using System.Collections;
using UnityEditor;

namespace cfUnityEngine.Editor
{
    public class CustomInspector : UnityEditor.Editor
    {
        public static object DrawField(string fieldName, object fieldValue)
        {
            var type = fieldValue?.GetType();
            if (type == typeof(UnityEngine.Vector2))
                return EditorGUILayout.Vector2Field(fieldName, (UnityEngine.Vector2)fieldValue);
            if(type == typeof(UnityEngine.Vector3))
                return EditorGUILayout.Vector3Field(fieldName, (UnityEngine.Vector3)fieldValue);
            if(type == typeof(bool))
                return EditorGUILayout.Toggle(fieldName, (bool)fieldValue);
            if(type == typeof(int))
                return EditorGUILayout.IntField(fieldName, (int)fieldValue);
            if(type == typeof(float))
                return EditorGUILayout.FloatField(fieldName, (float)fieldValue);
            if(type == typeof(string))
                return EditorGUILayout.TextField(fieldName, (string)fieldValue);
            if (type == typeof(Enum))
                return EditorGUILayout.EnumFlagsField(fieldName, (Enum)fieldValue);
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var dictionary = (IDictionary)fieldValue;
                if (dictionary != null)
                {
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        DrawField(entry.Key.ToString(), entry.Value);
                    }
                }
            }
            else
                EditorGUILayout.LabelField(fieldName, fieldValue != null ? fieldValue.ToString() : "null");

            return null;
        }
    }
}
