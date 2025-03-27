using System.Collections.Generic;
using cfEngine.Logging;
using UnityEngine;

namespace cfUnityEngine
{
    public class NamedPropertyBinder: PropertyBinder, INamespace
    {
        [SerializeField] private string _binderName;
        public string @namespace => _binderName;
        bool INamespaceResolver.isLeaf { get; set; } = true;

        private INamespaceResolver parent;
        
        private Dictionary<string, INamespace> _namespaceMap = new();

        protected override void Awake()
        {
            base.Awake();

            var pointer = transform;
            while (parent == null)
            {
                pointer = pointer.parent;
                if (pointer == null)
                    break;

                if (pointer.TryGetComponent<INamespaceResolver>(out var resolver))
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

        public void Attach(string nsName, INamespace ns)
        {
            if (!_namespaceMap.TryAdd(nsName, ns))
            {
                Log.LogWarning($"NamedPropertyBinder.Attach: namespace already exists, nsName: {nsName}");
            }
        }
    }
}