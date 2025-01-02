using System.Threading;
using cfEngine.Asset;
using cfEngine.Core;
using cfEngine.Core.Layer;
using cfEngine.Meta;
using cfEngine.Pooling;
using cfUnityEngine.GameState;
using Object = UnityEngine.Object;

public partial class Game
{
    public ref struct InitParam
    {
        public CancellationToken taskToken;
        public InfoLayer info;
        public AssetManager<Object> asset;
        public PoolManager pool;
        public GameStateMachine gsm;
        public LoginHandler auth;
        public UserDataManager userData;
    }
    
    public static CancellationToken TaskToken { get; private set; }
    public static InfoLayer Info { get; private set; }
    public static AssetManager<Object> Asset { get; private set; }
    public static PoolManager Pool { get; private set; }
    public static GameStateMachine Gsm { get; private set; }
    public static LoginHandler Auth { get; private set; }
    public static UserDataManager UserData { get; private set; }
    public static MetaLayer Meta { get; private set; }

    public static void MakeInstance(in InitParam param)
    {
        Info = param.info;
        Asset = param.asset;
        Pool = param.pool;
        Gsm = param.gsm;
        Auth = param.auth;
        TaskToken = param.taskToken;
        UserData = param.userData;
        Meta = new MetaLayer();
    }

    public static void Dispose()
    {
        Info?.Dispose(); Info = null;
        Asset?.Dispose(); Asset = null;
        Pool?.Dispose(); Pool = null;
        Gsm?.Dispose(); Gsm = null;
        Auth?.Dispose(); Auth = null;
        Meta?.Dispose(); Meta = null;
    }
}
