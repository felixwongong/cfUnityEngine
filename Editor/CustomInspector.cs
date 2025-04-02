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
            {
                fieldValue ??= default(UnityEngine.Vector2);
                return EditorGUILayout.Vector2Field(fieldName, (UnityEngine.Vector2)fieldValue);
            }
            else if(type == typeof(UnityEngine.Vector3))
            {
                fieldValue ??= default(UnityEngine.Vector3);
                return EditorGUILayout.Vector3Field(fieldName, (UnityEngine.Vector3)fieldValue);
            }
            else if(type == typeof(bool))
            {
                fieldValue ??= default(bool);
                return EditorGUILayout.Toggle(fieldName, (bool)fieldValue);
            }
            else if(type == typeof(int))
            {
                fieldValue ??= default(int);
                return EditorGUILayout.IntField(fieldName, (int)fieldValue);
            }
            else if(type == typeof(float))
            {
                fieldValue ??= default(float);
                return EditorGUILayout.FloatField(fieldName, (float)fieldValue);
            }
            else if(type == typeof(string))
            {
                fieldValue ??= default(string);
                return EditorGUILayout.TextField(fieldName, (string)fieldValue);
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
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
            {
                EditorGUILayout.LabelField(fieldName, fieldValue?.GetType().Name);
            }

            return null;
        }
    }
}
