using System;
using UnityEditor;

namespace cfUnityEngine.Editor
{
    public class CustomInspector : UnityEditor.Editor
    {
        public static object DrawField(Type type, string fieldName, object fieldValue)
        {
            //TODO: Use dictionary to improve performance
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
            else
            {
                EditorGUILayout.LabelField($"Field {type} not supported");
            }

            return null;
        }
    }
}
