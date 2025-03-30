namespace cfUnityEngine
{
    public class PropertyObjectBinder : PropertyBinderBase<PropertyObjectResolver, object>
    {
        protected override void AddObservers(IPropertySource source)
        {
            source.onPropertyChanged += OnSourcePropertyChanged;
        }

        protected override void RemoveObservers(IPropertySource source)
        {
            source.onPropertyChanged -= OnSourcePropertyChanged;
        }
    }
}