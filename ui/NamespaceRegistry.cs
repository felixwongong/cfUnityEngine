using System.Collections.Generic;
using cfEngine.Logging;

namespace cfUnityEngine
{
    public class NamespaceRegistry : PropertyBinder, INamespaceResolver
    {
        private Dictionary<string, INamespace> namespaceRegistry = new();
        public bool isLeaf { get; set; } = false;
        public void Attach(string nsName, INamespace ns)
        {
            if(!namespaceRegistry.TryAdd(nsName, ns))
            {
                Log.LogWarning($"NamespaceRegistry.Attach: namespace already exists, nsName: {nsName}");
            }
        }
    }
}