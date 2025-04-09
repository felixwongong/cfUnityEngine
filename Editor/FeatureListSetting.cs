using cfUnityEngine.Util;

namespace cfUnityEngine.Editor
{
    [FilePath("Assets/Settings", "FeatureListSetting")]
    public class FeatureListSetting: EditorSetting<FeatureListSetting>
    {
        public string[] features;
    }
}