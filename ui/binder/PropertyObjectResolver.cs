using System;
using UnityEngine;

namespace cfUnityEngine
{
    public abstract class PropertyResolverBase<TValueType> : MonoBehaviour
    {
        [SerializeField] protected string propertyName;
        [SerializeField] private bool useGameObjectName;

        private void Awake()
        {
            if (useGameObjectName)
            {
                propertyName = gameObject.name;
            }
        }

        public void Resolve(string resolveProperty, TValueType value)
        {
            if (resolveProperty.Equals(propertyName))
            {
                OnResolve(value);
            }
        }
        protected abstract void OnResolve(TValueType value);
    }

    public abstract class PropertyObjectResolver : PropertyResolverBase<object>, IPropertyObjectResolver
    {
        public bool canResolve { get => enabled; set => enabled = value; }
    }
    public abstract class PropertyBoolResolver: PropertyResolverBase<bool>, IPropertyBoolResolver
    {
        public bool canResolve { get => enabled; set => enabled = value; }
    }

    public abstract class PropertyIntResolver : PropertyResolverBase<int>, IPropertyIntResolver
    {
        public bool canResolve { get => enabled; set => enabled = value; }
    }

    public abstract class PropertyFloatResolver : PropertyResolverBase<float>, IPropertyFloatResolver
    {
        public bool canResolve { get => enabled; set => enabled = value; }
    }
}