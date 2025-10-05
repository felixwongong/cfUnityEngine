using cfEngine.Asset;
using cfEngine.Core;

namespace cfUnityEngine.Core
{
    public static partial class ServiceName
    {
        public const string Asset = "Asset";
    }
    
    public static partial class DomainExtension
    {
        public static Domain WithAsset(this Domain domain, AssetManager<UnityEngine.Object> service)
        {
            domain.Register(service, ServiceName.Asset);
            return domain;
        }
        
        public static AssetManager<UnityEngine.Object> GetAsset(this Domain domain) => domain.GetService<AssetManager<UnityEngine.Object>>(ServiceName.Asset);
    }
}
