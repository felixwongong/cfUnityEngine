using System;
using System.Diagnostics;

namespace cfUnityEngine.Util
{
    public class SerializedList
    {
        [Conditional("UNITY_EDITOR")]
        public class ReadOnlyAttribute : Attribute
        {
        }
    }
}