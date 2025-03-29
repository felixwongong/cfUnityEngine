namespace cfUnityEngine
{
    public class PropertyObjectBinder : PropertyBinderBase<IPropertyResolver, object>
    {
        private void OnSourcePropertyChanged(string propertyName, object value)
        {
            Resolve(resolvers, propertyName, value);
        }
        
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