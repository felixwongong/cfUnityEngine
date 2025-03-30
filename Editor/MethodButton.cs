using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace cfUnityEngine.Editor
{
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

            Component mono = target as Component;
            if (mono == null)
            {
                Debug.Log($"target {target} is not a Component!");
                return;
            }

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            MethodInfo[] methods = mono.GetType().GetMethods(flags);

            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                var methodButton = Attribute.GetCustomAttribute(methods[i], typeof(MethodButtonAttribute)) as MethodButtonAttribute;
                if (methodButton == null) continue;
                
                if(methodButton.isPlayModeOnly && !Application.isPlaying) continue;

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
                    if (type.IsValueType)
                    {
                        parameterValues[j] = Activator.CreateInstance(type);
                    }
                    else if(type == typeof(string))
                    {
                        parameterValues[j] = string.Empty;
                    }
                    parameterValues[j] = CustomInspector.DrawField(parameter.Name, parameterValues[j]);
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
