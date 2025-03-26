using System;
using System.Threading.Tasks;
using cfEngine.Asset;
using cfEngine.Core;
using cfEngine.Extension;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace cfUnityEngine.UI.UIToolkit
{
    public partial class UIPanel
    {
        public class Builder: IDisposable
        {
            private string _panelPath = null;
            private AssetManager<Object> _assetLoader = null;
            public Task<TemplateContainer> loadTask { get; private set; } = null;
            private string _name = null;

            public Builder SetAssetLoader(AssetManager<Object> assetLoader)
            {
                _assetLoader = assetLoader;
                return this;
            }

            public Builder SetPath(string panelPath)
            {
                _panelPath = panelPath;
                return this;
            }

            public Builder LoadTemplate()
            {
                var loadTaskSource = new TaskCompletionSource<TemplateContainer>();
                loadTask = loadTaskSource.Task;
                
                if (string.IsNullOrEmpty(_panelPath))
                {
                    loadTaskSource.SetException(new TemplateUninitializedException(this));
                    return this;
                }
                
                if(_assetLoader == null)
                {
                    loadTaskSource.SetException(new TemplateUninitializedException(this));
                    return this;
                }

                _assetLoader.LoadAsync<VisualTreeAsset>(_panelPath, Game.TaskToken)
                    .ContinueWithSynchronized(t =>
                    {
                        if (t.IsFaulted && t.Exception != null)
                        {
                            loadTaskSource.SetException(t.Exception);
                            return;
                        }

                        if (t.IsCanceled)
                        {
                            loadTaskSource.SetCanceled();
                            return;
                        }

                        var asset = t.Result;
                        var template = asset.Instantiate();
                        if (!string.IsNullOrEmpty(_name))
                        {
                            template.name = _name;
                        }
                        loadTaskSource.SetResult(template);
                    });
                
                return this;
            }

            public Builder SetName(string name)
            {
                _name = name;
                if (loadTask.IsCompletedSuccessfully)
                {
                    loadTask.Result.name = name;
                }

                return this;
            }
            
            public class TemplateUninitializedException : Exception
            {
                public TemplateUninitializedException(Builder builder) : base(createMessage(builder))
                {
                    
                }

                private static string createMessage(Builder builder)
                {
                    if (string.IsNullOrEmpty(builder._panelPath))
                        return $"Panel load task uninitialized due to panelPath unset, call {nameof(SetPath)} first";

                    if (builder.loadTask == null)
                        return $"Panel load task uninitialized, call {nameof(LoadTemplate)} first";
                    
                    if (builder._assetLoader == null)
                        return $"Panel load task uninitialized due to assetLoader unset, call {nameof(SetAssetLoader)} first";
                    
                    return $"Panel load task uninitialized";
                }
            }

            public void Dispose()
            {
                _assetLoader?.Dispose();
                loadTask?.DisposeIfCompleted();
            }
        }
    }
}