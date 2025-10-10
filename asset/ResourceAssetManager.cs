#if !CF_ADDRESSABLE

using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using cfEngine.Asset;
using cfEngine.Logging;
using UnityEngine;

namespace cfUnityEngine.Asset
{
    public class ResourceAssetManager : AssetManager<UnityEngine.Object>
    {
        private MonoBehaviour _mono;
        public ResourceAssetManager(string gameObjectName = nameof(ResourceAssetManager)): base()
        {
            var gameObject = new GameObject(gameObjectName)
            {
                hideFlags =
#if UNITY_EDITOR
                    HideFlags.DontSave |
#else
                    HideFlags.HideAndDontSave |
#endif
                    HideFlags.NotEditable
            }; 
            
            _mono = gameObject.AddComponent<MonoBehaviour>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }
        
        protected override AssetHandle<T> _Load<T>(string path)
        {
            var asset = Resources.Load<T>(path);
            if (asset == null)
            {
                throw new ArgumentException($"Asset not found ({path})", nameof(path));
            }

            return new AssetHandle<T>(asset, () => { });
        }

        protected override Task<AssetHandle<T>> _LoadAsync<T>(string path, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                Log.LogInfo($"Resource load {path} operation cancelled");
                return null;
            }

            try
            {
                var tcs = new TaskCompletionSource<AssetHandle<T>>();
                var req = Resources.LoadAsync<T>(path);

                _mono.StartCoroutine(WaitLoadRequest(req, tcs, token));
                IEnumerator WaitLoadRequest(ResourceRequest request, TaskCompletionSource<AssetHandle<T>> tcs, CancellationToken token)
                {
                    while (true)
                    {
                        if (token.IsCancellationRequested)
                        {
                            tcs.SetCanceled();
                            break;
                        }

                        if (request.isDone)
                        {
                            if (request.asset is not T t)
                            {
                                tcs.SetException(new IOException("Resource load failed"));
                            }
                            else
                            {
                                tcs.SetResult(new AssetHandle<T>(t, () => { }));
                            }
                            break;
                        }
                        
                        yield return null;
                    }
                }

                return tcs.Task;
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