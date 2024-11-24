using System;
using UnityEditor.Build;

namespace cfUnityEngine.Editor
{
    [Flags]
    public enum PlatformType
    {
        None = 0,
        Android = 1 << 0,
        IOS = 1 << 1,
        WebGL = 1 << 2,
        Standalone = 1 << 3,
    }

    public static class BuildTargetTypeExtension
    {
        public static NamedBuildTarget GetNamed(this PlatformType targetType)
        {
            return targetType switch
            {
                PlatformType.Standalone => NamedBuildTarget.Standalone,
                PlatformType.Android => NamedBuildTarget.Android,
                PlatformType.IOS => NamedBuildTarget.iOS,
                PlatformType.WebGL => NamedBuildTarget.WebGL,
                _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null)
            };
        }
    }
}