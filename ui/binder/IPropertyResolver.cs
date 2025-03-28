namespace cfUnityEngine
{
    public interface IPropertyResolver<in T>
    {
        void Resolve(string resolveProperty, T value);
    }
    
    public interface IPropertyResolver: IPropertyResolver<object> { }
    public interface IPropertyIntResolver: IPropertyResolver<int> { }
    public interface IPropertyFloatResolver: IPropertyResolver<float> { }
    public interface IPropertyBoolResolver : IPropertyResolver<bool> { }
}