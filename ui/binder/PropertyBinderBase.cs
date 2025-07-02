using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace cfUnityEngine
{
    public interface IPropertyBinder
    {
        void BindSource(IPropertySource source);
    }

    public abstract class PropertyBinderBase<T, TValueType> : MonoBehaviour, IPropertyBinder where T: MonoBehaviour, IPropertyResolver<TValueType>
    {
        private Dictionary<string, TValueType> _valueCacheMap = new();

        [SerializeField] protected List<T> resolvers;
        private IPropertySource source { get; set; }

        public void BindSource(IPropertySource newSource)
        {
            if (source != null)
            {
                RemoveObservers(source);
            }
            
            source = newSource;
            AddObservers(newSource);
        }
        
        private void OnDestroy()
        {
            if (source != null)
            {
                RemoveObservers(source);
            }
        }
        
        protected void OnSourcePropertyChanged(string propertyName, TValueType value)
        {
            Resolve(propertyName, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Resolve(string propertyName, TValueType value)
        {
            _valueCacheMap[propertyName] = value;
            
            for (var i = 0; i < resolvers.Count; i++)
            {
                var resolver = resolvers[i];
                if (resolver is { canResolve: true })
                {
                    resolver.Resolve(propertyName, value);
                }
            }
        }

        protected abstract void AddObservers(IPropertySource source);

        protected abstract void RemoveObservers(IPropertySource source);
    }
}
