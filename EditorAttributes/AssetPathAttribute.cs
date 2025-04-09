using System;
using System.Diagnostics;
using UnityEngine;

namespace cfUnityEngine.Editor
{
    [Conditional("UNITY_EDITOR"), AttributeUsage(AttributeTargets.Field)]
    public class AssetPathAttribute : PropertyAttribute
    {
    }
}