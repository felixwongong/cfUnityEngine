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
        public abstract IReadOnlyList<TInfo> getInfos { get; }
    }
    
    public class EditorScriptableObjectLoader<TInfo>: IValueLoader<TInfo>
    {
        private readonly string _folderPath;
        private readonly string _searchPattern;

        public EditorScriptableObjectLoader(string folderPath, string searchPattern = "*")
        {
            _folderPath = folderPath;
            _searchPattern = searchPattern;
        }
        
        public ObjectPool<List<TInfo>>.Handle Load(out List<TInfo> values)
        {
            var handle = ListPool<TInfo>.Default.Get(out values);

            var guids = AssetDatabase.FindAssets(_searchPattern, new [] {_folderPath});
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var scriptableInfo = AssetDatabase.LoadAssetAtPath<ScriptableInfo<TInfo>>(assetPath);
                
                if (scriptableInfo == null)
                {
                    Log.LogException(new ArgumentException($"[ScriptableObjectLoader] Info Load failed, type: {typeof(TInfo).Name}"));
                    return handle;
                }

                var infos = scriptableInfo.getInfos;
                if (infos != null)
                {
                    values.EnsureCapacity(values.Count + infos.Count);
                    values.AddRange(infos);
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

                var infos = scriptableInfo.getInfos;
                if (infos != null)
                {
                    list.EnsureCapacity(list.Count + infos.Count);
                    list.AddRange(infos);
                }
            }
            
            return Task.FromResult(list);
        }
    }
}