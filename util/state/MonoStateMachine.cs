using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using cfEngine.Logging;
using cfEngine.Rx;
using cfEngine.Util;
using UnityEngine;

namespace cfUnityEngine.Util
{
    public abstract class MonoStateMachine<TStateId, TState, TStateMachine> : MonoBehaviour, IStateMachine<TStateId>
        where TStateMachine : MonoStateMachine<TStateId, TState, TStateMachine>
        where TState: MonoState<TStateId, TState, TStateMachine>
    {
        protected TState lastState { get; private set; }
        protected TState currentState { get; private set; }
        public TStateId lastStateId => lastState.Id;
        public TStateId currentStateId => currentState.Id;

        private readonly Dictionary<TStateId, TState> _stateDictionary = new();
        protected IEnumerable<TState> allState => _stateDictionary.Values;

        #region Relay & Events (OnBeforeStateChange[Once], OnAfterStateChange[Once]);

        private Relay<StateChangeRecord<TStateId>> _beforeStateChangeRelay;
        private Relay<StateChangeRecord<TStateId>> _afterStateChangeRelay;
        
        public Subscription SubscribeBeforeStateChange(Action<StateChangeRecord<TStateId>> listener)
        {
            _beforeStateChangeRelay ??= new Relay<StateChangeRecord<TStateId>>(this);
            return _beforeStateChangeRelay.AddListener(listener);
        }
        
        public Subscription SubscribeAfterStateChange(Action<StateChangeRecord<TStateId>> listener)
        {
            _afterStateChangeRelay ??= new Relay<StateChangeRecord<TStateId>>(this);
            return _afterStateChangeRelay.AddListener(listener);
        }

        #endregion


        private void Awake()
        {
            _Awake();

            var states = GetComponents<TState>();
            foreach (var state in states)
            {
                RegisterState(state);
                state.enabled = false;
            }

            foreach (var state in states)
            {
                state._Awake();
            }
        }

        protected virtual void _Awake()
        {
        }

        private void Start()
        {
            _Start();
            foreach (var state in _stateDictionary.Values)
            {
                state._Start();
            }
        }

        protected virtual void _Start()
        {
        }

        private void Update()
        {
            _Update();
            if (currentState != null && currentState.CanUpdate())
            {
                currentState._Update();
            }
        }

        protected virtual void _Update()
        {
        }

        public virtual void StartMachine()
        {
            
        }

        public void RegisterState([NotNull] TState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            if (!_stateDictionary.TryAdd(state.Id, state))
            {
                throw new Exception($"State {state.GetType()} already registered");
            }

            state.SetStateMachine((TStateMachine)this);
        }

        public bool CanGoToState(TStateId id)
        {
            return TryGetState(id, out var nextState) && nextState.IsReady();
        }

        public bool TryGoToState(TStateId nextStateId, in StateParam param = null)
        {
            try
            {
                if (!TryGetState(nextStateId, out var nextState))
                {
                    Log.LogException(new KeyNotFoundException($"State {nextStateId} not registered"));
                    return false;
                }

                if (!CanGoToState(nextState.Id))
                {
                    Log.LogException(new ArgumentException(
                        $"Cannot go to state {nextState.Id}, not in current state {currentState.Id} whitelist"));
                    return false;
                }

                if (currentState != null)
                {
                    _beforeStateChangeRelay?.Dispatch(new StateChangeRecord<TStateId>
                        { LastState = currentState.Id, NewState = nextState.Id });

                    currentState.OnEndContext();
                    currentState.enabled = false;
                    lastState = currentState;
                }

                currentState = nextState;
                currentState.enabled = true;
                if (lastState != null)
                {
                    _afterStateChangeRelay?.Dispatch(new StateChangeRecord<TStateId>
                        { LastState = lastState.Id, NewState = currentState.Id });
                }
                currentState.StartContext(param);
                return true;
            }
            catch (Exception ex)
            {
                Log.LogException(new StateExecutionException<TStateId>(nextStateId, ex));
                return false;
            }
        }

        public void ForceGoToState(TStateId nextStateId, in StateParam param = null)
        {
            try
            {
                if (!TryGetState(nextStateId, out var nextState))
                {
                    Log.LogException(new KeyNotFoundException($"State {nextStateId} not registered"));
                    return;
                }

                if (currentState != null)
                {
                    _beforeStateChangeRelay?.Dispatch(new StateChangeRecord<TStateId>
                        { LastState = currentState.Id, NewState = nextState.Id });

                    currentState.OnEndContext();
                    currentState.enabled = false;
                    lastState = currentState;
                }

                currentState = nextState;
                currentState.enabled = true;
                if (lastState != null)
                {
                    _afterStateChangeRelay?.Dispatch(new StateChangeRecord<TStateId>
                        { LastState = lastState.Id, NewState = currentState.Id });
                }
                currentState.StartContext(param);
            }
            catch (Exception ex)
            {
                Log.LogException(new StateExecutionException<TStateId>(nextStateId, ex));
            }
        }

        public TState GetStateUnsafe(TStateId id)
        {
            if (!_stateDictionary.TryGetValue(id, out var state))
            {
                Log.LogException(new Exception($"State {id} not registered"));
                return null;
            }

            return state;
        }

        public T GetStateUnsafe<T>(TStateId id) where T : TState
        {
            return (T)GetStateUnsafe(id);
        }

        public bool TryGetState(TStateId id, out TState monoState)
        {
            if (!_stateDictionary.TryGetValue(id, out monoState))
            {
                Log.LogException(new Exception($"State {id} not registered"));
                return false;
            }

            return true;
        }

        public bool TryGetState<T>(TStateId id, out T state) where T : TState
        {
            state = null;

            if (TryGetState(id, out var monoState) && monoState is T t)
            {
                state = t;
                return true;
            }

            return false;
        }
    }

    public class StateExecutionException<TStateId> : Exception
    {
        public StateExecutionException(TStateId stateId, Exception innerException) : base(
            $"State {stateId} execution failed", innerException)
        {
        }
    }
}