using System;
using System.Collections.Generic;
using cfEngine.Input;
using cfEngine.Rx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace cfUnityEngine.Input
{
    public class InputSystem : MonoBehaviour, IInputSystem
    {
        private PlayerInput _playerInput;
        private Dictionary<string, Relay<IInputActionContext>> _actionRelays = new();

        private Relay<IInputActionContext> _onActionTriggered;
        private Relay _onDeviceLost;
        private Relay _onDeviceRegained;
        private Relay<string> _onControlsChanged;

        public IRelay<Action<IInputActionContext>> onActionTriggered => _onActionTriggered;
        public IRelay<Action> onDeviceLost => _onDeviceLost;
        public IRelay<Action> onDeviceRegained => _onDeviceRegained;
        public IRelay<Action<string>> onControlsChanged => _onControlsChanged;

        public string currentControlScheme => _playerInput?.currentControlScheme;
        public string currentActionMap => _playerInput?.currentActionMap?.name;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            if (_playerInput == null)
            {
                Debug.LogError("[InputSystem] PlayerInput component not found!");
                return;
            }

            InitializeRelays();
        }

        private void InitializeRelays()
        {
            _onActionTriggered = new Relay<IInputActionContext>(this);
            _onDeviceLost = new Relay(this);
            _onDeviceRegained = new Relay(this);
            _onControlsChanged = new Relay<string>(this);
        }

        private void OnEnable()
        {
            if (_playerInput != null)
            {
                SubscribeToPlayerInput();
            }
        }

        private void OnDisable()
        {
            if (_playerInput != null)
            {
                UnsubscribeFromPlayerInput();
            }
        }

        private void SubscribeToPlayerInput()
        {
            _playerInput.onActionTriggered += HandleActionTriggered;
            _playerInput.onDeviceLost += HandleDeviceLost;
            _playerInput.onDeviceRegained += HandleDeviceRegained;
            _playerInput.onControlsChanged += HandleControlsChanged;
        }

        private void UnsubscribeFromPlayerInput()
        {
            _playerInput.onActionTriggered -= HandleActionTriggered;
            _playerInput.onDeviceLost -= HandleDeviceLost;
            _playerInput.onDeviceRegained -= HandleDeviceRegained;
            _playerInput.onControlsChanged -= HandleControlsChanged;
        }

        private void HandleActionTriggered(InputAction.CallbackContext context)
        {
            InputActionContext actionContext;
            
            string actionName = context.action.name;
            if (!context.canceled)
            {
                
                object value = context.ReadValueAsObject();
                actionContext = new InputActionContext(actionName, value, IInputActionContext.Phase.Performed);
            }
            else
            {
                actionContext = new InputActionContext(actionName, null, IInputActionContext.Phase.Canceled);
            }

            _onActionTriggered.Dispatch(actionContext);

            if (_actionRelays.TryGetValue(actionName, out var relay))
            {
                relay.Dispatch(actionContext);
            }
        }

        private void HandleDeviceLost(PlayerInput input)
        {
            _onDeviceLost.Dispatch();
        }

        private void HandleDeviceRegained(PlayerInput input)
        {
            _onDeviceRegained.Dispatch();
        }

        private void HandleControlsChanged(PlayerInput input)
        {
            _onControlsChanged.Dispatch(input.currentControlScheme);
        }

        public Subscription RegisterAction(string actionName, Action<IInputActionContext> callback)
        {
            if (!_actionRelays.TryGetValue(actionName, out var relay))
            {
                relay = new Relay<IInputActionContext>(this);
                _actionRelays[actionName] = relay;
            }

            return relay.AddListener(callback);
        }

        public void UnregisterAction(string actionName, Action<IInputActionContext> callback)
        {
            if (_actionRelays.TryGetValue(actionName, out var relay))
            {
                relay.RemoveListener(callback);
            }
        }

        public T GetActionValue<T>(string actionName) where T : struct
        {
            if (_playerInput == null)
                return default;

            var action = _playerInput.currentActionMap?.FindAction(actionName);
            return action?.ReadValue<T>() ?? default;
        }

        public bool IsActionPerformed(string actionName)
        {
            if (_playerInput == null)
                return false;

            var action = _playerInput.currentActionMap?.FindAction(actionName);
            return action != null && action.phase == InputActionPhase.Performed;
        }

        public void Dispose()
        {
            UnsubscribeFromPlayerInput();

            _onActionTriggered?.RemoveAll();
            _onDeviceLost?.RemoveAll();
            _onDeviceRegained?.RemoveAll();
            _onControlsChanged?.RemoveAll();

            foreach (var relay in _actionRelays.Values)
            {
                relay.RemoveAll();
            }

            _actionRelays.Clear();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}
