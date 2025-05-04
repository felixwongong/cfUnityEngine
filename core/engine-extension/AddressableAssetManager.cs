#if CF_ADDRESSABLE

using System;
using System.Threading;
using System.Threading.Tasks;
using cfEngine.Asset;
using cfEngine.Extension;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace cfUnityEngine.Asset
{
    public class AddressableAssetManager: AssetManager<Object>
    {
        protected override AssetHandle<T> _Load<T>(string key)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(key);
                handle.WaitForCompletion();

                return new AssetHandle<T>(handle.Result.GetComponent<T>(), handle.Release);
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<T>(key);
                handle.WaitForCompletion();

                return new AssetHandle<T>(handle.Result, handle.Release);
            }
        }

        protected override Task<AssetHandle<T>> _LoadAsync<T>(string key, CancellationToken token = default)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                var handle = _LoadAsync<GameObject>(key, token);
                return handle.ContinueWith(result =>
                {
                    if (result?.Result?.Asset != null && result.Result.Asset.TryGetTarget(out var go) && go.TryGetComponent<T>(out var comp))
                    {
                        return new AssetHandle<T>(comp, result.DisposeIfCompleted);
                    }
                    
                    return new AssetHandle<T>(null, result.DisposeIfCompleted);
                });
            }
            
            TaskCompletionSource<AssetHandle<T>> loadTaskSource = new();
            {
                var handle = Addressables.LoadAssetAsync<T>(key);
                
                if (token.IsCancellationRequested)
                {
                    handle.Release();
                    loadTaskSource.SetCanceled();
                }

                handle.Completed += OnLoadCompleted;
            }

            void OnLoadCompleted(AsyncOperationHandle<T> handle)
            {
                handle.Completed -= OnLoadCompleted;

                switch (handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        loadTaskSource.SetResult(new AssetHandle<T>(handle.Result, handle.Release));
                        break;
                    case AsyncOperationStatus.Failed:
                        loadTaskSource.SetException(handle.OperationException);
                        break;
                    case AsyncOperationStatus.None:
                        loadTaskSource.SetException(new Exception($"Asset {key} load completed triggered with Status None"));
                        break;
                }
            }

            return loadTaskSource.Task;
        }
    }
}

#endif