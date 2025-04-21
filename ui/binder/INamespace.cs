using System;

namespace cfUnityEngine
{
    public interface INamespaceScope
    {
        public string @namespace { get; }
        public bool isLeaf { get; set; }
        public void Attach(string nsName, INamespaceScope ns);
        public void BindSource(IPropertySource source);
        public INamespaceScope BindChildSource(string childName, IPropertySource source);
    }
}