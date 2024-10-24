using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using cfEngine.Logging;
using cfEngine.Util;
using UnityEngine;

namespace cfUnityEngine.Util
{
    public abstract class MonoStateMachine<TStateId, TState, TStateMachine> : MonoBehaviour, IStateMachine<TStateId>
        where TStateMachine : MonoStateMachine<TStateId, TState, TStateMachine>
        where TState: MonoState<TStateId, TState, TStateMachine>
    {
        private TState _lastState;
        private TState _currentState;
        public TStateId LastStateId => _lastState.Id;
        public TStateId CurrentStateId => _currentState.Id;

        private readonly Dictionary<TStateId, TState> _stateDictionary = new();

        public event Action<StateChangeRecord<TStateId>> OnBeforeStateChange;
        public event Action<StateChangeRecord<TStateId>> OnAfterStateChange;

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
            if (_currentState != null)
            {
                _currentState._Update();
            }
        }

        protected virtual void _Update()
        {
        }

        public void RegisterState([NotNull] TState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            if (!_stateDictionary.TryAdd(state.Id, state))
            {
                throw new Exception($"State {state.GetType()} already registered");
            }
        }

        public bool CanGoToState(TStateId id)
        {
            return TryGetState(id, out _) && (_currentState == null || _currentState.Whitelist.Contains(id));
        }

        public void GoToState(TStateId nextStateId, in StateParam param = null, bool checkWhitelist = true)
        {
            try
            {
                if (!TryGetState(nextStateId, out var nextState))
                {
                    Log.LogException(new KeyNotFoundException($"State {nextStateId} not registered"));
                    return;
                }

                if (checkWhitelist && !CanGoToState(nextState.Id))
                {
                    Log.LogException(new ArgumentException(
                        $"Cannot go to state {nextState.Id}, not in current state {_currentState.Id} whitelist"));
                    return;
                }

                if (_currentState != null)
                {
                    OnBeforeStateChange?.Invoke(new StateChangeRecord<TStateId>
                        { LastState = _currentState.Id, NewState = nextState.Id });

                    _currentState.OnEndContext();
                    _currentState.enabled = false;
                    _lastState = _currentState;
                }

                nextState.enabled = true;
                nextState.StartContext((TStateMachine)this, param);
                _currentState = nextState;

                OnAfterStateChange?.Invoke(new StateChangeRecord<TStateId>
                    { LastState = _currentState.Id, NewState = nextState.Id });
            }
            catch (Exception ex)
            {
                Log.LogException(new StateExecutionException<TStateId>(nextStateId, ex));
            }
        }

        public TState GetState(TStateId id)
        {
            if (!_stateDictionary.TryGetValue(id, out var state))
            {
                Log.LogException(new Exception($"State {id} not registered"));
                return null;
            }

            return state;
        }

        public T GetState<T>(TStateId id) where T : TState
        {
            return (T)GetState(id);
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