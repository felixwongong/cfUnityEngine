#if CF_GOOGLE_DRIVE

using System;
using cfUnityEngine.Util;
using UnityEngine;

namespace cfUnityEngine.GoogleDrive
{
    [FilePath("Assets/GoogleDrive", "MirrorSetting")]
    public class GDriveMirrorSetting : EditorSetting<GDriveMirrorSetting>
    {
        public TextAsset serviceAccountCredentialJson;
        public float refreshIntervalSecond = 5f;
        public MirrorItem[] items;
    }

    [Serializable]
    public class MirrorItem
    {
        public string googleDriveId;
        public string mirrorAssetLocation;
    }
}

#endif
