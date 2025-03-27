using System;
using System.Collections.Generic;

namespace cfUnityEngine
{
    public interface INamespace: INamespaceResolver
    {
        public string @namespace { get; }
    }
    
    public interface INamespaceResolver
    {
        public bool isLeaf { get; set; }
        public void Attach(string nsName, INamespace ns);
    }
}