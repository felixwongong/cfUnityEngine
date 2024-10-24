    using UnityEngine;

    public abstract class MonoInstance<T> : MonoBehaviour where T : Component
    {
        public virtual bool persistent => false;
        
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                _instance = FindAnyObjectByType<T>();

                if (_instance != null) return _instance;
                
                _instance = new GameObject($"_{typeof(T).Name}").AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if(persistent) DontDestroyOnLoad(this);
        }
    }
