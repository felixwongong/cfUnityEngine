using System.Collections.Generic;
using cfEngine.Util;
using UnityEngine;

namespace cfUnityEngine.Util
{
    public abstract class MonoState<TStateId, TState, TStateMachine>: MonoBehaviour 
        where TStateMachine: MonoStateMachine<TStateId, TState, TStateMachine> 
        where TState : MonoState<TStateId, TState, TStateMachine>
    {
        public abstract TStateId Id { get; }
        public virtual HashSet<TStateId> Whitelist { get; } = new HashSet<TStateId>();
        
        public TStateMachine StateMachine { get; internal set; }

        private void Awake() { }

        private void Start() { }

        private void Update() { }

        public virtual void _Awake() { }

        public virtual void _Start() { }

        public virtual void _Update() { }

        public virtual bool IsReady() => true;
        public virtual bool CanUpdate() => true;
        
        protected internal abstract void StartContext(StateParam param);

        protected internal virtual void OnEndContext()
        {
        }
    }
}