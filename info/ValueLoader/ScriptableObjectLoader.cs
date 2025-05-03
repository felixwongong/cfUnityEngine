using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cfEngine.Asset;
using cfEngine.Extension;
using cfEngine.Info;
using cfEngine.Logging;
using cfEngine.Pooling;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace cfUnityEngine.Info
{
    public class ScriptableInfo<TInfo> : ScriptableObject
    {
        public List<TInfo> infos;
    }
    
    public class ScriptableObjectLoader<TInfo>: IValueLoader<TInfo>
    {
        private readonly string _path;
        private readonly AssetManager<Object> _assetManager;

        public ScriptableObjectLoader(string path, AssetManager<Object> assetManager)
        {
            _path = path;
            _assetManager = assetManager;
            
            Assert.IsNotNull(_assetManager);
        }
        
        public ObjectPool<List<TInfo>>.Handle Load(out List<TInfo> values)
        {
            var handle = ListPool<TInfo>.Default.Get(out values);
            var scriptableInfo = _assetManager.Load<ScriptableInfo<TInfo>>(_path);
            if (scriptableInfo == null)
            {
                Log.LogException(new ArgumentException($"[ScriptableObjectLoader] Info Load failed, type: {typeof(TInfo).Name}"));
                return handle;
            }

            if (scriptableInfo.infos != null)
            {
                values.EnsureCapacity(scriptableInfo.infos.Count);
                values.AddRange(scriptableInfo.infos);
            }
            return handle;
        }

        public async Task<List<TInfo>> LoadAsync(CancellationToken cancellationToken)
        {
            var list = ListPool<TInfo>.Default.Get();
            var scriptableInfo = await _assetManager.LoadAsync<ScriptableInfo<TInfo>>(_path, cancellationToken);
            
            if (scriptableInfo == null)
            {
                Log.LogException(new ArgumentException($"[ScriptableObjectLoader] Info Load failed, type: {typeof(TInfo).Name}"));
                return list;
            }

            if (scriptableInfo.infos != null)
            {
                list.EnsureCapacity(scriptableInfo.infos.Count);
                list.AddRange(scriptableInfo.infos);
            }
            return list;
        }
    }
}