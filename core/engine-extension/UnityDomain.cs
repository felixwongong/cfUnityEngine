using cfEngine.Core;
using cfUnityEngine.Util;
using UnityEngine;

namespace cfUnityEngine.Core
{
    public class UnityDomain: Domain
    {
        public GameObject domainObject { get; private set; }
        
        public UnityDomain(string gameObjectName = nameof(UnityDomain)): base()
        {
            domainObject = new GameObject(gameObjectName)
            {
                hideFlags =
#if UNITY_EDITOR
                    HideFlags.DontSave |
#else
                    HideFlags.HideAndDontSave |
#endif
                    HideFlags.NotEditable
                    
            };
            
            GameObjectUtil.DontDestroyOnLoadIfRoot(domainObject);
        }
    }
}