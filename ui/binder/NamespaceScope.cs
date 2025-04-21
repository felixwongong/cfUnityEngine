using System;
using System.Collections.Generic;
using cfEngine.Logging;
using UnityEngine;

namespace cfUnityEngine
{
    public class NamespaceScope : MonoBehaviour, INamespaceScope
    {
        public bool isLeaf { get; set; } = true;
        public string @namespace => _binderName;
        [SerializeField] private string _binderName;
        
        private INamespaceScope _parent;
        private Dictionary<string, INamespaceScope> _namespaceMap = new();
        
        protected void Awake()
        {
            var pointer = transform;
            while (_parent == null)
            {
                pointer = pointer.parent;
                if (pointer == null)
                    break;

                if (pointer.TryGetComponent<INamespaceScope>(out var resolver))
                {
                    _parent = resolver;
                    _parent.isLeaf = false;
                    break;
                }
            }
        }

        private void Start()
        {
            _parent?.Attach(_binderName, this);
        }
        
        public void Attach(string nsName, INamespaceScope ns)
        {
            if(!_namespaceMap.TryAdd(nsName, ns))
            {
                Log.LogWarning($"NamespaceRegistry.Attach: namespace already exists, nsName: {nsName}");
            }
        }

        private INamespaceScope GetChild(string nsName)
        {
            return _namespaceMap.GetValueOrDefault(nsName);
        }

#if UNITY_EDITOR
        private Dictionary<string, (IPropertySource, bool)> __cachedSources = new();
#endif
        public void BindSource(IPropertySource source)
        {
#if UNITY_EDITOR
            __cachedSources[@namespace] = (source, true);
#endif
            if (TryGetScopeComponents<IPropertyBinder>(out var binders))
            {
                foreach (var binder in binders.Span)
                {
                    binder.BindSource(source);
                }
            }
        }

        public INamespaceScope BindChildSource(string childName, IPropertySource source)
        {
            var child = GetChild(childName);
            child?.BindSource(source);

#if UNITY_EDITOR
            __cachedSources[childName] = (source, child != null);
#endif
            
            return child;
        }
        
        private bool TryGetScopeComponents<T>(out ReadOnlyMemory<T> components)
        {
            components = GetComponents<T>();
            return components.Length > 0;
        }
    }
}