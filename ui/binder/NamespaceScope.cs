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
        
        private INamespaceScope parent;
        private Dictionary<string, INamespaceScope> _namespaceMap = new();
        
        protected void Awake()
        {
            var pointer = transform;
            while (parent == null)
            {
                pointer = pointer.parent;
                if (pointer == null)
                    break;

                if (pointer.TryGetComponent<INamespaceScope>(out var resolver))
                {
                    parent = resolver;
                    parent.isLeaf = false;
                    break;
                }
            }
        }

        private void Start()
        {
            parent?.Attach(_binderName, this);
        }
        
        public void Attach(string nsName, INamespaceScope ns)
        {
            if(!_namespaceMap.TryAdd(nsName, ns))
            {
                Log.LogWarning($"NamespaceRegistry.Attach: namespace already exists, nsName: {nsName}");
            }
        }

        public INamespaceScope GetSubspace(string nsName)
        {
            return _namespaceMap.GetValueOrDefault(nsName);
        }

        public ReadOnlyMemory<T> GetScopeComponents<T>()
        {
            return GetComponents<T>();
        }

        public bool TryGetScopeComponents<T>(out ReadOnlyMemory<T> components)
        {
            components = GetComponents<T>();
            return components.Length > 0;
        }

        public void SetBinderSource(IPropertySource source)
        {
            if (TryGetScopeComponents<IPropertyBinder>(out var binders))
            {
                foreach (var binder in binders.Span)
                {
                    binder.BindSource(source);
                }
            }
        }
    }
}