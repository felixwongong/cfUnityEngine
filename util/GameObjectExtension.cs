using System.Runtime.CompilerServices;
using UnityEngine;

namespace cfUnityEngine.Util
{
    public static class GameObjectUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DontDestroyOnLoadIfRoot(GameObject go)
        {
            if (go.transform.parent == null)
            {
                Object.DontDestroyOnLoad(go);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        
        public static void DontDestroyOnLoadIfRoot(Component go)
        {
            if (go.transform.parent == null)
            {
                Object.DontDestroyOnLoad(go);
            }
        }
    }
}