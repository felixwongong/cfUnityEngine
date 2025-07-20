using cfEngine.Asset;
using cfEngine.Core;

namespace cfUnityEngine.Core
{
    public static partial class ServiceName
    {
        public const string Asset = "Asset";
    }
    
    public static partial class GameExtension
    {
        public static Game WithAsset(this Game game, AssetManager<UnityEngine.Object> service)
        {
            game.Register(service, ServiceName.Asset);
            return game;
        }
        
        public static AssetManager<UnityEngine.Object> GetAsset(this Game game) => game.GetService<AssetManager<UnityEngine.Object>>(ServiceName.Asset);
    }
}
