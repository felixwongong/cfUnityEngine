using System.Collections.Generic;
using UnityEngine;

namespace cfUnityEngine
{
    public abstract class PropertyResolverGroup<TResolver>: MonoBehaviour where TResolver : IPropertyResolver
    {
    }
}