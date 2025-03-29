namespace cfUnityEngine
{
    public class PropertyFloatBinder: PropertyBinderBase<IPropertyFloatResolver, float>
    {
        private void OnSourcePropertyChanged(string propertyName, float value)
        {
            Resolve(resolvers, propertyName, value);
        }
        
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