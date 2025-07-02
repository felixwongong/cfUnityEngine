using System;
using cfEngine.Logging;
using cfEngine.Rx;

namespace cfUnityEngine
{
    public interface IPropertySource
    {
        event Action<string, object> onPropertyChanged;
        event Action<string, int> onPropertyIntChanged;
        event Action<string, float> onPropertyFloatChanged;
        event Action<string, bool> onPropertyBoolChanged;
    }

    public class PropertySource: IPropertySource
    {
        private Relay<string, object> onPropertyChanged;

        event Action<string, object> IPropertySource.onPropertyChanged
        {
            add { onPropertyChanged ??= new Relay<string, object>(this); onPropertyChanged.AddListener(value); }
            remove => onPropertyChanged?.RemoveListener(value);
        }

        private Relay<string, int> onPropertyIntChanged;

        event Action<string, int> IPropertySource.onPropertyIntChanged
        {
            add { onPropertyIntChanged ??= new Relay<string, int>(this); onPropertyIntChanged.AddListener(value); }
            remove => onPropertyIntChanged?.RemoveListener(value);
        }

        private Relay<string, float> onPropertyFloatChanged;

        event Action<string, float> IPropertySource.onPropertyFloatChanged
        {
            add { onPropertyFloatChanged ??= new Relay<string, float>(this); onPropertyFloatChanged.AddListener(value); }
            remove => onPropertyFloatChanged?.RemoveListener(value);
        }

        private Relay<string, bool> onPropertyBoolChanged;
        event Action<string, bool> IPropertySource.onPropertyBoolChanged
        {
            add { onPropertyBoolChanged ??= new Relay<string, bool>(this); onPropertyBoolChanged.AddListener(value); }
            remove => onPropertyBoolChanged?.RemoveListener(value);
        }

        public void OnPropertyChanged(string propertyName, int value)
        {
            onPropertyIntChanged?.Dispatch(propertyName, value);
        }
        
        public void OnPropertyChanged(string propertyName, float value)
        {
            onPropertyFloatChanged?.Dispatch(propertyName, value);
        }
        
        public void OnPropertyChanged(string propertyName, bool value)
        {
            onPropertyBoolChanged?.Dispatch(propertyName, value);
        }

        public void OnPropertyChanged(string propertyName, object value)
        {
            if (value == null)
            {
                Log.LogError($"PropertySource: Attempt to dispatch null value for property '{propertyName}'");
                return;
            }

            onPropertyChanged?.Dispatch(propertyName, value);
        }

        public void OnPropertyChanged(string propertyName, Action action)
        {
            OnPropertyChanged(propertyName, (object)action);
        }
    }
}