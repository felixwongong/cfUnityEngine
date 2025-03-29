namespace cfUnityEngine
{
    public class PropertyIntBinder: PropertyBinderBase<IPropertyIntResolver, int>
    {
        private void OnSourcePropertyChanged(string propertyName, int value)
        {
            Resolve(resolvers, propertyName, value);
        }
        
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