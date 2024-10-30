using System;
using System.Diagnostics;
using System.Threading;
using cfEngine.Asset;
using cfEngine.Core;
using cfEngine.Core.Layer;
using cfEngine.IO;
using cfEngine.Logging;
using cfEngine.Pooling;
using cfEngine.Serialize;
using cfEngine.Util;
using cfUnityEngine.GameState;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameEntry : MonoBehaviour
{
    [Conditional("UNITY_EDITOR")]
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void PreprocessInEditor()
    {
        try
        {
            Game.InfoBuildByte();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Log.SetLogger(new UnityLogger());
        Log.SetLogLevel(LogLevel.Debug);

        var cts = new CancellationTokenSource();
        var initParam = new Game.InitParam()
        {
            taskToken = cts.Token,
            info = new InfoLayer(new StreamingAssetStorage("Info"), JsonSerializer.Instance),

#if CF_ADDRESSABLE
            asset = new AddressableAssetManager(),
#else
            asset = new ResourceAssetManager(),
#endif
            pool = new PoolManager(),
            gsm = new GameStateMachine(),
            userData = new UserDataManager(new FileStorage(Application.persistentDataPath), JsonSerializer.Instance),
            auth = new LocalLoginHandler()
        };
        
        Game.MakeInstance(initParam);
        
        Game.Auth.RegisterPlatform(new LocalPlatform());
        Game.UserData.RegisterSavables();
        
        Game.Gsm.OnAfterStateChange += OnStateChanged;
        Application.quitting += OnApplicationQuit;
        
        void OnApplicationQuit()
        {
            Game.Gsm.OnAfterStateChange -= OnStateChanged;
            Application.quitting -= OnApplicationQuit;
            
            cts.Cancel();
            Game.Dispose();
        }
        
        Game.Gsm.GoToState(GameStateId.InfoLoad);
    }

    private static void OnStateChanged(StateChangeRecord<GameStateId> record)
    {
        Log.LogInfo($"Game state changed, {record.LastState.ToString()} -> {record.NewState.ToString()}");
    }

    private void Start()
    {
        Game.Gsm.OnAfterStateChange += OnBootstrapEnd;
        void OnBootstrapEnd(StateChangeRecord<GameStateId> record)
        {
            if (record.NewState != GameStateId.BootstrapEnd)
                return;
            
            Game.Gsm.OnAfterStateChange -= OnBootstrapEnd;
        }
    }
}
