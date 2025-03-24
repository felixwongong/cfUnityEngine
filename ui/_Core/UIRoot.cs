using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cfEngine.Asset;
using cfEngine.Core;
using cfEngine.Extension;
using cfEngine.Logging;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace cfUnityEngine.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIRoot : MonoBehaviour, IDisposable
    {
#if UNITY_EDITOR
        private const string EditorName = "{0} (EditorName)";
#endif

        public static UIRoot Instance { get; private set; }

        public class PanelConfig
        {
            public string path;
        }

        [SerializeField] private UIDocument uiRootDocument;

        private Dictionary<Type, PanelConfig> _registeredPanelMap = new();
        private Dictionary<Type, Task<TemplateContainer>> _templateLoadMap = new();
        private Dictionary<Type, UIPanel> _panelMap = new();
        
        private Dictionary<Type, UIPanel.Builder> _panelBuilderMap = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiRootDocument == null)
            {
                uiRootDocument = GetComponent<UIDocument>();
            }
        }
#endif

        public void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Register<T>(string panelPath) where T : UIPanel
        {
            var type = typeof(T);

            if (!_panelBuilderMap.TryAdd(type, new UIPanel.Builder().SetPath(panelPath)))
            {
                Log.LogException(new ArgumentException($"UIRoot.Register: panel type already registered, type: {type}, path: {panelPath}"));
            }
        }

        public Task<TemplateContainer> LoadTemplate<T>() where T : UIPanel
        {
            var type = typeof(T);
            if (!_registeredPanelMap.TryGetValue(type, out var config))
            {
                var ex = new KeyNotFoundException($"{nameof(LoadTemplate)}: panel type not registered: {type}");
                Log.LogException(ex);
                return Task.FromException<TemplateContainer>(ex);
            }

            if (_templateLoadMap.TryGetValue(type, out var templateLoadTask) &&
                templateLoadTask.IsCompletedSuccessfully)
            {
                return templateLoadTask;
            }

            TaskCompletionSource<TemplateContainer> loadTaskSource = new();
            _templateLoadMap[type] = loadTaskSource.Task;

            Game.Current.GetAsset<Object>().LoadAsync<VisualTreeAsset>(config.path, Game.TaskToken)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted && t.Exception != null)
                    {
                        loadTaskSource.SetException(t.Exception);
                        return;
                    }

                    var template = t.Result.Instantiate();
#if UNITY_EDITOR
                    template.name = string.Format(EditorName, type);
#endif
                    loadTaskSource.SetResult(template);
                }, Game.TaskToken, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            return loadTaskSource.Task;
        }

        public async Task<T> LoadPanel<T>() where T : UIPanel
        {
            var template = await LoadTemplate<T>();
            var panel = (T)Activator.CreateInstance(typeof(T));
            panel.AttachVisual(template);
            _panelMap.Add(typeof(T), panel);

            AttachTemplateToRoot(template);
            panel.HidePanel();
            return panel;
        }

        public void AttachTemplateToRoot(TemplateContainer template)
        {
            if (template == null)
            {
                Log.LogException(new ArgumentNullException(nameof(template)));
                return;
            }

            uiRootDocument.rootVisualElement.Add(template);
            template.StretchToParentSize();
        }

        public static T GetPanel<T>() where T : UIPanel
        {
            var type = typeof(T);
            if (!Instance._registeredPanelMap.ContainsKey(type))
            {
                Log.LogException(
                    new KeyNotFoundException(
                        $"UI.GetPanel: panel type not registered: {type}, call {nameof(Register)} first"));
                return null;
            }

            if (!Instance._panelMap.TryGetValue(type, out var panel))
            {
                Log.LogException(
                    new KeyNotFoundException(
                        $"UI.GetPanel: panel type not loaded: {type}, call {nameof(LoadPanel)} first"));
                return null;
            }

            if (panel is not T t)
            {
                Log.LogException(
                    new InvalidCastException(
                        $"UI.GetPanel: panel type mismatch: load: {panel.GetType()}, Request: {type}"));
                return null;
            }

            return t;
        }

        public void Dispose()
        {
            _registeredPanelMap.Clear();

            foreach (var task in _templateLoadMap.Values)
            {
                task.DisposeIfCompleted();
            }

            _templateLoadMap.Clear();

            foreach (var panel in _panelMap.Values)
            {
                panel.Dispose();
            }

            _panelMap.Clear();

            uiRootDocument.rootVisualElement?.Clear();
        }
    }
}