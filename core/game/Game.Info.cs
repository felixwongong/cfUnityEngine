using System.Collections.Generic;
using System.Diagnostics;
using cfEngine.Core.Layer;
using cfEngine.Info;
using cfEngine.Serialize;

public partial class Game
{
    [Conditional("UNITY_EDITOR")]
    public static void InfoBuildByte()
    {
        var editorLayer = new InfoLayer(new EditorAssetStorage("Info"), JsonSerializer.Instance);

        foreach (var info in InfoLayer.infos)
        {
            editorLayer.RegisterInfo(info);
            info.DirectlyLoadFromExcel();
        }
        
        var runtimeLayer = new InfoLayer(new StreamingAssetStorage("Info"), JsonSerializer.Instance);
        
        foreach (var info in InfoLayer.infos)
        {
            runtimeLayer.RegisterInfo(info);
            info.SerializeIntoStorage();
        }
    }
}