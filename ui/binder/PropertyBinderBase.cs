using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace cfUnityEngine
{
    public interface IPropertyBinder
    {
        void BindSource(IPropertySource source);
    }

    public abstract class PropertyBinderBase<T, TValueType> : MonoBehaviour, IPropertyBinder
    {
#if UNITY_EDITOR
        [UnityEngine.Scripting.Preserve] private TValueType __cachedValue;
#endif
        
        protected List<T> resolvers { get; private set; }
        private IPropertySource source { get; set; }

        protected virtual void Awake()
        {
            var res = GetComponents<T>();
            if(res.Length > 0) resolvers = new List<T>(res);
        }
        
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Resolve<TResolver, TValue>(IReadOnlyList<TResolver> resolvers, string propertyName, TValue value) where TResolver: IPropertyResolver<TValue> where TValue: TValueType
        {
#if UNITY_EDITOR
            __cachedValue = value;
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
