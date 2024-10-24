using System;
using System.Threading;
using System.Threading.Tasks;
using cfEngine.Logging;
using UnityEngine;

namespace cfEngine.Asset
{
public class ResourceAssetManager : AssetManager<UnityEngine.Object>
{
    protected override AssetHandle<T> _Load<T>(string path)
    {
        var asset = Resources.Load<T>(path);
        if (asset == null)
        {
            throw new ArgumentException($"Asset not found ({path})", nameof(path));
        }

        return new AssetHandle<T>(asset, () => {});
    }

    protected override async Task<AssetHandle<T>> _LoadAsync<T>(string path, CancellationToken token)
    {
        if (token.WaitHandle != null)
        {
            Log.LogWarning("Resources Load can't really be cancelled");
        }

        try
        {
            var req = Resources.LoadAsync<T>(path);
            await req;

            var t = (T)req.asset;
            return new AssetHandle<T>(t, () => { });
        }
        catch (Exception e)
        {
            Log.LogException(e, $"Resource {path} load failed.");
            return null;
        }
    }
}
}