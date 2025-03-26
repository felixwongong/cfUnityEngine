using System.Collections.Generic;
using UnityEngine;

namespace cfUnityEngine
{
    public class PropertyBinder : MonoBehaviour
    {
        private IPropertySource source;
        
        private IPropertyResolver[] resolvers;
        private IPropertyIntResolver[] intResolvers;
        private IPropertyFloatResolver[] floatResolvers;

        protected virtual void Awake()
        {
            resolvers = GetComponents<IPropertyResolver>();
            intResolvers = GetComponents<IPropertyIntResolver>();
            floatResolvers = GetComponents<IPropertyFloatResolver>();
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
            RemoveObservers(source);
        }

        private void OnSourcePropertyChanged(string propertyName, object value)
        {
            if(resolvers == null) return;
            
            for (var i = 0; i < resolvers.Length; i++)
            {
                resolvers[i].Resolve(propertyName, value);
            }
        }

        private void OnSourcePropertyIntChanged(string propertyName, int value)
        {
            if(intResolvers == null) return;

            for (var i = 0; i < intResolvers.Length; i++)
            {
                intResolvers[i].Resolve(propertyName, value);
            }
        }
        
        private void OnSourcePropertyFloatChanged(string propertyName, float value)
        {
            if(floatResolvers == null) return;
            
            for (var i = 0; i < floatResolvers.Length; i++)
            {
                floatResolvers[i].Resolve(propertyName, value);
            }
        }
        
        private void AddObservers(IPropertySource source)
        {
            if (source != null)
            {
                source.onPropertyChanged += OnSourcePropertyChanged;
                source.onPropertyIntChanged += OnSourcePropertyIntChanged;
                source.onPropertyFloatChanged += OnSourcePropertyFloatChanged;
            }
        }

        private void RemoveObservers(IPropertySource source)
        {
            if (source != null)
            {
                source.onPropertyChanged -= OnSourcePropertyChanged;
                source.onPropertyIntChanged -= OnSourcePropertyIntChanged;
                source.onPropertyFloatChanged -= OnSourcePropertyFloatChanged;
            }
        }
    }
}
