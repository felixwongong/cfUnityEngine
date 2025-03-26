namespace cfUnityEngine
{
    public interface IPropertyResolver
    {
        void Resolve(string propertyName, object value);
    }
    
    public interface IPropertyIntResolver
    {
        void Resolve(string propertyName, int value);
    }
    
    public interface IPropertyFloatResolver
    {
        void Resolve(string propertyName, float value);
    }
}