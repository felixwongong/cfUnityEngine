using System;
using cfEngine.Logging;

namespace cfUnityEngine
{
    public interface IPropertySource
    {
        public event Action<string, object> onPropertyChanged;
        public event Action<string, int> onPropertyIntChanged;
        public event Action<string, float> onPropertyFloatChanged;
    }

    public class PropertySource: IPropertySource
    {
        public event Action<string, object> onPropertyChanged;
        public event Action<string, int> onPropertyIntChanged;
        public event Action<string, float> onPropertyFloatChanged;

        protected void OnPropertyChanged<T>(string propertyName, T value)
        {
            if (value is int intValue)
            {
                onPropertyIntChanged?.Invoke(propertyName, intValue);
            } else if (value is float floatValue)
            {
                onPropertyFloatChanged?.Invoke(propertyName, floatValue);
            }
            else 
            {
                if (value.GetType().IsValueType)
                {
                    Log.LogWarning($"PropertySource.OnPropertyChange: passing value type {typeof(T).FullName} might cause boxing, suggest use class");
                }
                
                onPropertyChanged?.Invoke(propertyName, value);
            }
        }
    }
}