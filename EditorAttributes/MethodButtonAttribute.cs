using System;
using System.Diagnostics;

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
}