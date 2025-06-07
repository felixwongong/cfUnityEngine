using System.Diagnostics;
using UnityEngine;

namespace cfUnityEngine.Editor
{
    [Conditional("UNITY_EDITOR")]
    public class ReadOnlyAttribute: PropertyAttribute
    {
    }
}