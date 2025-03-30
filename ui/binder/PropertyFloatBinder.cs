namespace cfUnityEngine
{
    public class PropertyFloatBinder: PropertyBinderBase<PropertyFloatResolver, float>
    {
        protected override void AddObservers(IPropertySource source)
        {
            source.onPropertyFloatChanged += OnSourcePropertyChanged;
        }

        protected override void RemoveObservers(IPropertySource source)
        {
            source.onPropertyFloatChanged -= OnSourcePropertyChanged;
        }
    }
}