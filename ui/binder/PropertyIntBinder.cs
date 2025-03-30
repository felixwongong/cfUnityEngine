namespace cfUnityEngine
{
    public class PropertyIntBinder: PropertyBinderBase<PropertyIntResolver, int>
    {
        protected override void AddObservers(IPropertySource source)
        {
            source.onPropertyIntChanged += OnSourcePropertyChanged;
        }

        protected override void RemoveObservers(IPropertySource source)
        {
            source.onPropertyIntChanged -= OnSourcePropertyChanged;
        }
    }
}