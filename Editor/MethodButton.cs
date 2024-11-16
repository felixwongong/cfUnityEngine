using System;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace cfUnityEngine.Editor
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodButtonAttribute : Attribute
    {
        public string buttonName { get; set; }

        public MethodButtonAttribute(string btnName = "")
        {
            buttonName = btnName;
        }
    }


    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MethodButtonEditor : UnityEditor.Editor
    {
        private GUIStyle _headerStyle;
        private object[] parameterValues;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            }

            MonoBehaviour mono = target as MonoBehaviour;
            if (mono == null)
            {
                Debug.Log($"target {target} is not a MonoBehaviour!");
                return;
            }

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo[] methods = mono.GetType().GetMethods(flags);

            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                var methodButton =
                    Attribute.GetCustomAttribute(methods[i], typeof(MethodButtonAttribute)) as MethodButtonAttribute;

                if (methodButton == null) continue;

                if (string.IsNullOrEmpty(methodButton.buttonName))
                {
                    methodButton.buttonName = method.Name;
                }

                GUILayout.FlexibleSpace();
                GUILayout.Label(methodButton.buttonName, _headerStyle);

                var parameters = method.GetParameters();
                parameterValues ??= new object[parameters.Length];

                for (int j = 0; j < parameters.Length; j++)
                {
                    var parameter = parameters[j];
                    EditorGUILayout.BeginHorizontal();
                    Type type = parameter.ParameterType;
                    parameterValues[j] = CustomInspector.DrawField(type, parameter.Name, parameterValues[j]);
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Invoke " + methodButton.buttonName))
                {
                    method.Invoke(mono, parameterValues);
                }
            }
        }
    }
}
