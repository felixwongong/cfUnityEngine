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

namespace cfUnityEngine.UI.UIToolkit
{
    [RequireComponent(typeof(UIDocument))]
    public class UIToolkitRoot : MonoBehaviour, IDisposable
    {
#if UNITY_EDITOR
        private const string EditorName = "{0} (EditorName)";
#endif

        public static UIToolkitRoot Instance { get; private set; }

        public class PanelConfig
        {
            public string path;
        }

        [SerializeField] private UIDocument uiRootDocument;

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

            if (!_panelBuilderMap.TryAdd(type, new UIPanel.Builder().SetAssetLoader(Game.Current.GetAsset<Object>()).SetPath(panelPath)))
            {
                Log.LogException(new ArgumentException($"UIRoot.Register: panel type already registered, type: {type}, path: {panelPath}"));
            }
        }

        public Task<TemplateContainer> LoadTemplate<T>() where T : UIPanel
        {
            var type = typeof(T);
            if (!_panelBuilderMap.TryGetValue(type, out var builder))
            {
                var ex = new KeyNotFoundException($"{nameof(LoadTemplate)}: panel type not registered: {type}");
                Log.LogException(ex);
                return Task.FromException<TemplateContainer>(ex);
            }

            return builder.LoadTemplate().SetName(string.Format(EditorName, typeof(T))).loadTask;
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
            if (!Instance._panelBuilderMap.ContainsKey(type))
            {
                Log.LogException(
                    new KeyNotFoundException( $"UI.GetPanel: panel type not registered: {type}, call {nameof(Register)} first"));
                return null;
            }

            if (!Instance._panelMap.TryGetValue(type, out var panel))
            {
                Log.LogException(new KeyNotFoundException( $"UI.GetPanel: panel type not loaded: {type}, call {nameof(LoadPanel)} first"));
                return null;
            }

            if (panel is not T t)
            {
                Log.LogException(new InvalidCastException($"UI.GetPanel: panel type mismatch: load: {panel.GetType()}, Request: {type}"));
                return null;
            }

            return t;
        }

        public void Dispose()
        {
            foreach (var builder in _panelBuilderMap.Values)
            {
                builder.Dispose();
            }

            foreach (var panel in _panelMap.Values)
            {
                panel.Dispose();
            }

            _panelMap.Clear();

            uiRootDocument.rootVisualElement?.Clear();
        }
    }
}