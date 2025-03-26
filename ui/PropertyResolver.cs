using UnityEngine;

namespace cfUnityEngine
{
    public abstract class PropertyResolver : MonoBehaviour, IPropertyResolver
    {
        [SerializeField] private string propertyName;
        
        public void Resolve(string propertyName, object value)
        {
            if (propertyName.Equals(this.propertyName))
            {
                OnResolve(value);
            }
        }
        
        protected abstract void OnResolve(object value);
    }
}