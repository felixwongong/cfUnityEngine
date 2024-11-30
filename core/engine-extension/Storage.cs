using System.IO;
using cfEngine.IO;
using UnityEngine;

public class StreamingAssetStorage : FileStorage
{
    public StreamingAssetStorage(string subDirectory) : base(Path.Combine(Application.dataPath, "StreamingAssets", subDirectory))
    {
        
    }
}

public class EditorAssetStorage : FileStorage
{
    public EditorAssetStorage(string subDirectory) : base(Path.Combine(Application.dataPath, subDirectory))
    {
    }
}