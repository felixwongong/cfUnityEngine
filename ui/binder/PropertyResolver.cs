using UnityEngine;

namespace cfUnityEngine
{
    public abstract class PropertyResolver : MonoBehaviour, IPropertyResolver
    {
        [SerializeField] private string propertyName;
        
        public void Resolve(string resolveProperty, object value)
        {
            if (resolveProperty.Equals(this.propertyName))
            {
                OnResolve(value);
            }
        }
        
        protected abstract void OnResolve(object value);
    }
}