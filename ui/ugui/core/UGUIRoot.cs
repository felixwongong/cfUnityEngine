using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfEngine.Asset;
using cfEngine.Extension;
using UnityEngine;
using cfEngine.Logging;
using Object = UnityEngine.Object;

namespace cfUnityEngine.UI.UGUI
{
    public class UGUIRoot: MonoBehaviour, IUIRoot
    {
        private AssetManager<Object> _assetLoader;
        private Dictionary<string, UIPanel.IBuilder> _panelBuilders = new();

        public void Initialize(AssetManager<Object> assetLoader)
        {
            _assetLoader = assetLoader;
        }
        
        public T Register<T>(T panel, string path) where T : IUIPanel
        {
            var panelId = panel.id;
            if (!_panelBuilders.TryAdd(panelId, new UIPanel.Builder<T>().SetDataSource(panel).SetPath(path).SetAssetLoader(_assetLoader)))
            {
                Log.LogException(new ArgumentException($"UIRoot.Register: panel already registered, type: {panelId}, path: {path}"));
            }

            return panel;
        }

        public async Task PreloadPanel(string panelId)
        {
            if (!_panelBuilders.TryGetValue(panelId, out var builder))
            {
                Log.LogException(new ArgumentException($"UIRoot.PreloadPanel: panel not registered, panelId: {panelId}"));
                return;
            }
            
            await builder.Preload();
        }

        public Task InstantiatePanel(string panelId)
        {
            if(!_panelBuilders.TryGetValue(panelId, out var builder))
            {
                Log.LogException(new ArgumentException($"UIRoot.InstantiatePanel: panel not registered, panelId: {panelId}"));
                return Task.CompletedTask;
            }
            
            var task = builder.Instantiate();
            return task.ContinueWithSynchronized(t => 
            {
                if (t.IsFaulted)
                {
                    Log.LogException(t.Exception);
                }
                else
                {
                    var instanceTransform = t.Result.transform;
                    instanceTransform.SetParent(transform);
                    instanceTransform.localPosition = Vector3.zero;
                }
            });
        }

        public IUIPanel GetPanel(string panelId)
        {
            if (!_panelBuilders.TryGetValue(panelId, out var builder))
            {
                Log.LogException(new ArgumentException($"UIRoot.GetPanel: panel not registered, panelId: {panelId}"));
                return default;
            }

            return (IUIPanel)builder.GetDataSource();
        }

        public T GetPanel<T>(string panelId) where T : IUIPanel
        {
            if (!_panelBuilders.TryGetValue(panelId, out var builder))
            {
                Log.LogException(new ArgumentException($"UIRoot.GetPanel: panel not registered, panelId: {panelId}"));
                return default;
            }

            return (T)builder.GetDataSource();
        }

        public void Dispose()
        {
            _assetLoader?.Dispose();
        }
    }
}