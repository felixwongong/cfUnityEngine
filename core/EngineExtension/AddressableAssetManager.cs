using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace cfEngine.Asset
{
    public class AddressableAssetManager: AssetManager<Object>
    {
        protected override AssetHandle<T> _Load<T>(string key)
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            handle.WaitForCompletion();

            return new AssetHandle<T>(handle.Result, handle.Release);
        }

        protected override Task<AssetHandle<T>> _LoadAsync<T>(string key, CancellationToken token)
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            
            TaskCompletionSource<AssetHandle<T>> loadTaskSource = new();
            
            if (token.IsCancellationRequested)
            {
                handle.Release();
                loadTaskSource.SetCanceled();
            }

            handle.Completed += OnLoadCompleted;

            void OnLoadCompleted(AsyncOperationHandle<T> _)
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