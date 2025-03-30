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
#if UNITY_EDITOR
        [UnityEngine.Scripting.Preserve] private Dictionary<string, TValueType> _cachedValueMap = new();
#endif
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
            Resolve(resolvers, propertyName, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Resolve<TResolver, TValue>(IReadOnlyList<TResolver> resolvers, string propertyName, TValue value) where TResolver: IPropertyResolver<TValue> where TValue: TValueType
        {
#if UNITY_EDITOR
            _cachedValueMap[propertyName] = value;
#endif
            
            for (var i = 0; i < resolvers.Count; i++)
            {
                resolvers[i].Resolve(propertyName, value);
            }
        }

        protected abstract void AddObservers(IPropertySource source);

        protected abstract void RemoveObservers(IPropertySource source);
    }
}
