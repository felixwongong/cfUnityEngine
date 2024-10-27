using System;
using UnityEditor.Build;

namespace cfUnityEngine.Editor
{
    [Flags]
    public enum BuildTargetType
    {
        None = 0,
        Android = 1 << 0,
        IOS = 1 << 1,
        WebGL = 1 << 2,
        Standalone = 1 << 3,
    }

    public static class BuildTargetTypeExtension
    {
        public static NamedBuildTarget GetNamed(this BuildTargetType targetType)
        {
            return targetType switch
            {
                BuildTargetType.Standalone => NamedBuildTarget.Standalone,
                BuildTargetType.Android => NamedBuildTarget.Android,
                BuildTargetType.IOS => NamedBuildTarget.iOS,
                BuildTargetType.WebGL => NamedBuildTarget.WebGL,
                _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null)
            };
        }
    }
}