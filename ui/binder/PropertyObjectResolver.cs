using UnityEngine;

namespace cfUnityEngine
{
    public abstract class PropertyResolverBase<TValueType> : MonoBehaviour
    {
        [SerializeField] private string propertyName;

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