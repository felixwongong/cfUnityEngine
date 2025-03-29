using UnityEngine;

namespace cfUnityEngine.UI.UGUI
{
    public class ActiveInactiveResolver: MonoBehaviour, IPropertyBoolResolver
    {
        [SerializeField] private string propertyName = "enabled";
            
        public void Resolve(string resolveProperty, bool value)
        {
            if (resolveProperty.Equals(propertyName))
            {
                SetActive(value);
            }
        }
        
        [Editor.MethodButton]
        public virtual void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}