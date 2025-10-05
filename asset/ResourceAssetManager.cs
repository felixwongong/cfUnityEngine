#if !CF_ADDRESSABLE

using System;
using System.Threading;
using System.Threading.Tasks;
using cfEngine.Asset;
using cfEngine.Logging;
using UnityEngine;

namespace cfUnityEngine.Asset
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

            return new AssetHandle<T>(asset, () => { });
        }

        protected override async Task<AssetHandle<T>> _LoadAsync<T>(string path, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                Log.LogInfo($"Resource load {path} operation cancelled");
                return null;
            }

            try
            {
                var req = Resources.LoadAsync<T>(path);
                var taskSource = new TaskCompletionSource<bool>();
                req.completed += OnLoadCompleted;

                void OnLoadCompleted(AsyncOperation aop)
                {
                    req.completed -= OnLoadCompleted;
                    taskSource.SetResult(true);
                }
                await taskSource.Task;

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
#endif