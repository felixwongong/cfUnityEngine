    using System;
    using UnityEngine;

    public abstract class MonoInstance<T> : MonoBehaviour where T : Component
    {
        public virtual bool dontDestroyOnLoad => false;
        public static Func<T> createMethod => () => new GameObject($"_{typeof(T).Name}").AddComponent<T>();

        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                _instance = FindAnyObjectByType<T>();

                if (_instance != null) return _instance;

                _instance = createMethod();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if(dontDestroyOnLoad) DontDestroyOnLoad(this);
        }
    }
