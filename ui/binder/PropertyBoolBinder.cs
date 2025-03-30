namespace cfUnityEngine
{
    public class PropertyBoolBinder: PropertyBinderBase<PropertyBoolResolver, bool>
    {
        protected override void AddObservers(IPropertySource source)
        {
            source.onPropertyBoolChanged += OnSourcePropertyChanged;
        }

        protected override void RemoveObservers(IPropertySource source)
        {
            source.onPropertyBoolChanged -= OnSourcePropertyChanged;
        }
    }
}