using System;

namespace cfUnityEngine
{
    public interface INamespaceScope
    {
        public string @namespace { get; }
        public bool isLeaf { get; set; }
        public void Attach(string nsName, INamespaceScope ns);
        public void SetSource(IPropertySource source);
        public INamespaceScope SetChildSource(string childName, IPropertySource source);
    }
}