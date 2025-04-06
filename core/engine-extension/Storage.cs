using System.IO;
using cfEngine.IO;
using UnityEngine;

public class StreamingAssetStorage : LocalFileStorage
{
    public StreamingAssetStorage(string subDirectory) : base(Path.Combine(Application.streamingAssetsPath, subDirectory))
    {
        
    }
}

public class EditorAssetStorage : LocalFileStorage
{
    public EditorAssetStorage(string subDirectory) : base(Path.Combine(Application.dataPath, subDirectory))
    {
    }
}