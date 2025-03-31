namespace cfUnityEngine
{
    public interface IPropertyResolver<in T>
    {
        bool canResolve { get; set; }
        void Resolve(string resolveProperty, T value);
    }
    
    public interface IPropertyObjectResolver: IPropertyResolver<object> { }
    public interface IPropertyIntResolver: IPropertyResolver<int> { }
    public interface IPropertyFloatResolver: IPropertyResolver<float> { }
    public interface IPropertyBoolResolver : IPropertyResolver<bool> { }
}