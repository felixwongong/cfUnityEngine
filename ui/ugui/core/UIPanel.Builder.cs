using System;
using System.Threading.Tasks;
using cfEngine.Asset;
using cfEngine.Extension;
using cfEngine.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace cfUnityEngine.UI.UGUI
{
    public abstract partial class UIPanel
    {
        public interface IBuilder
        {
            IBuilder SetPath(string path);
            public IBuilder SetAssetLoader(AssetManager<Object> assetLoader);
            public Task Preload();
            Task<GameObject> Instantiate();
        }

        public class Builder<T> : IBuilder where T: IUIPanel
        {
            private string _path;
            private T _dataSource;
            private AssetManager<Object> _assetLoader;
            private Task<GameObject> _preloadTask;
            
            public IBuilder SetPath(string path)
            {
                _path = path;
                return this;
            }

            public Builder<T> SetDataSource(T dataSource)
            {
                _dataSource = dataSource;
                return this;
            }
            
            public IBuilder SetAssetLoader(AssetManager<Object> assetLoader)
            {
                _assetLoader = assetLoader;
                return this;
            }

            public Task Preload()
            {
                if (_assetLoader == null)
                {
                    Log.LogWarning($"UIPanel.Builder.Preload: asset loader is null, call SetAssetLoader first, path: {_path}");
                    return Task.CompletedTask;
                }
                
                _preloadTask = _assetLoader.LoadAsync<GameObject>(_path);
                return _preloadTask;
            }
            
            public Task<GameObject> Instantiate()
            {
                if (_preloadTask == null)
                {
                    Log.LogWarning($"UIPanel.Builder.Instantiate: preload task is null, call Preload first, path: {_path}");
                    Preload();
                }
                
                TaskCompletionSource<GameObject> promise = new();
               _preloadTask.ContinueWithSynchronized(task =>
                    {
                        if (task.IsFaulted && task.Exception != null)
                        {
                            promise.SetException(task.Exception);
                        }
                        else if(task.IsCanceled)
                        {
                            promise.SetCanceled();
                        }
                        else
                        {
                            var prefab = task.Result;
                            var instance = Object.Instantiate(prefab);
                            if (!instance.TryGetComponent<INamespaceScope>(out var scope))
                            {
                                promise.SetException(new InvalidOperationException("UIPanel.Builder.Instantiate: property binder not found on instantiated panel, cannot bind"));
                            }
                            else
                            {
                                try
                                {
                                    _dataSource.Bind(scope);
                                    promise.SetResult(instance);
                                }
                                catch (Exception e)
                                {
                                    promise.SetException(e);
                                }
                            }
                        }
                    });
                
                return promise.Task;
            }
        }
    }
}