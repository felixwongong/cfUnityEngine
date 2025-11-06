using System.Collections.Generic;
using cfEngine.Util;
using UnityEngine;

namespace cfUnityEngine.Util
{
    public abstract class MonoState<TStateId, TState, TStateMachine>: MonoBehaviour 
        where TStateMachine: MonoStateMachine<TStateId, TState, TStateMachine> 
        where TState : MonoState<TStateId, TState, TStateMachine>
    {
        public abstract TStateId id { get; }
        protected TStateMachine stateMachine { get; private set; }
        
        [SerializeField] private GameObject[] stateObjects;
        
        public void SetStateMachine(TStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        private void Awake() { }

        private void Start() { }

        private void Update() { }

        public virtual void _Awake()
        {
            if (stateObjects != null)
            {
                foreach (var stateObject in stateObjects)
                {
                    stateObject.SetActive(false);
                }
            }
        }

        public virtual void _Start() { }

        public virtual void _Update() { }
        public virtual void _FixedUpdate() { }

        public virtual bool IsReady(StateParam param) => true;
        public virtual bool CanUpdate() => true;

        internal void StartContext(StateParam param)
        {
            if (stateObjects != null)
            {
                foreach (var stateObject in stateObjects)
                {
                    stateObject.SetActive(true);
                }
            }
            _StartContext(param);
        }
        
        protected abstract void _StartContext(StateParam param);

        internal void OnEndContext()
        {
            _OnEndContext();
            if (stateObjects != null)
            {
                foreach (var stateObject in stateObjects)
                {
                    stateObject.SetActive(false);
                }
            }
        }
        
        protected virtual void _OnEndContext()
        {
        }
    }
}