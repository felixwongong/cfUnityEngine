using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cfEngine.Asset;
using cfEngine.Extension;
using cfEngine.Info;
using cfEngine.Logging;
using cfEngine.Pooling;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace cfUnityEngine.Info
{
    public abstract class ScriptableInfo<TInfo> : ScriptableObject
    {
        public abstract List<TInfo> infos { get; }
    }
    
    public class EditorScriptableObjectLoader<TInfo>: IValueLoader<TInfo>
    {
        private readonly string _folderPath;

        public EditorScriptableObjectLoader(string folderPath)
        {
            _folderPath = folderPath;
        }
        
        public ObjectPool<List<TInfo>>.Handle Load(out List<TInfo> values)
        {
            var handle = ListPool<TInfo>.Default.Get(out values);

            var guids = AssetDatabase.FindAssets("*", new [] {_folderPath});
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var scriptableInfo = AssetDatabase.LoadAssetAtPath<ScriptableInfo<TInfo>>(assetPath);
                
                if (scriptableInfo == null)
                {
                    Log.LogException(new ArgumentException($"[ScriptableObjectLoader] Info Load failed, type: {typeof(TInfo).Name}"));
                    return handle;
                }

                if (scriptableInfo.infos != null)
                {
                    values.EnsureCapacity(values.Count + scriptableInfo.infos.Count);
                    values.AddRange(scriptableInfo.infos);
                }
            }
            
            return handle;
        }

        public Task<List<TInfo>> LoadAsync(CancellationToken cancellationToken)
        {
            var list = ListPool<TInfo>.Default.Get();
            var guids = AssetDatabase.FindAssets(_folderPath);
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var scriptableInfo = AssetDatabase.LoadAssetAtPath<ScriptableInfo<TInfo>>(assetPath);
                
                if (scriptableInfo == null)
                {
                    Log.LogException(new ArgumentException($"[ScriptableObjectLoader] Info Load failed, type: {typeof(TInfo).Name}"));
                    continue;
                }

                if (scriptableInfo.infos != null)
                {
                    list.EnsureCapacity(list.Count + scriptableInfo.infos.Count);
                    list.AddRange(scriptableInfo.infos);
                }
            }
            
            return Task.FromResult(list);
        }
    }
}