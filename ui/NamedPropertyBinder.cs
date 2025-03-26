using UnityEngine;

namespace cfUnityEngine
{
    public class NamedPropertyBinder: PropertyBinder
    {
        [SerializeField] private string @namespace;
    }
}