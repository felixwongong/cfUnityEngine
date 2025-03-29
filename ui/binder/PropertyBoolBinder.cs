namespace cfUnityEngine
{
    public class PropertyBoolBinder: PropertyBinderBase<IPropertyBoolResolver, bool>
    {
        private void OnSourcePropertyChanged(string propertyName, bool value)
        {
            Resolve(resolvers, propertyName, value);
        }
        
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