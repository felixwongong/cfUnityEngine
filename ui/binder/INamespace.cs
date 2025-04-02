using System;

namespace cfUnityEngine
{
    public interface INamespaceScope
    {
        public string @namespace { get; }
        public bool isLeaf { get; set; }
        public void Attach(string nsName, INamespaceScope ns);
        public INamespaceScope GetSubspace(string nsName);
        public ReadOnlyMemory<T> GetScopeComponents<T>();
        public bool TryGetScopeComponents<T>(out ReadOnlyMemory<T> components);
        public void SetBinderSource(IPropertySource source);
    }
}