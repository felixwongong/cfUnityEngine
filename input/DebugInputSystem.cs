using System;
using System.Collections.Generic;
using cfEngine.Input;
using cfEngine;
using cfEngine.Rx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace cfUnityEngine.Input
{
    public class DebugInputSystem : MonoBehaviour, IInputSystem
    {
        [Header("Debug Settings")]
        [SerializeField] private bool logInputEvents = true;
        [SerializeField] private bool showVisualFeedback = true;

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
                Log.LogError("[DebugInputSystem] PlayerInput component not found! Please attach PlayerInput to this GameObject.");
                return;
            }

            if (_playerInput.notificationBehavior != PlayerNotifications.InvokeCSharpEvents)
            {
                Log.LogWarning($"[DebugInputSystem] PlayerInput notification behavior is set to {_playerInput.notificationBehavior}. " +
                              "This system is designed for 'Invoke C# Events' behavior.");
            }

            InitializeRelays();

            if (logInputEvents)
            {
                Log.LogInfo($"[DebugInputSystem] Initialized. Current Action Map: {currentActionMap}");
            }
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

            if (logInputEvents)
            {
                Log.LogInfo("[DebugInputSystem] Subscribed to PlayerInput events.");
            }
        }

        private void UnsubscribeFromPlayerInput()
        {
            _playerInput.onActionTriggered -= HandleActionTriggered;
            _playerInput.onDeviceLost -= HandleDeviceLost;
            _playerInput.onDeviceRegained -= HandleDeviceRegained;
            _playerInput.onControlsChanged -= HandleControlsChanged;

            if (logInputEvents)
            {
                Log.LogInfo("[DebugInputSystem] Unsubscribed from PlayerInput events.");
            }
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

            if (logInputEvents)
            {
                string phase = context.phase.ToString();
                string controlPath = context.control?.path ?? "Unknown";
                Log.LogInfo($"[DebugInputSystem] Action: {actionName} | Phase: {phase} | Value: {context.ReadValueAsObject()} | Control: {controlPath}");
            }
        }

        private void HandleDeviceLost(PlayerInput input)
        {
            _onDeviceLost.Dispatch();

            if (logInputEvents)
            {
                Log.LogWarning($"[DebugInputSystem] Device Lost for player {input.playerIndex}");
            }
        }

        private void HandleDeviceRegained(PlayerInput input)
        {
            _onDeviceRegained.Dispatch();

            if (logInputEvents)
            {
                Log.LogInfo($"[DebugInputSystem] Device Regained for player {input.playerIndex}");
            }
        }

        private void HandleControlsChanged(PlayerInput input)
        {
            _onControlsChanged.Dispatch(input.currentControlScheme);

            if (logInputEvents)
            {
                Log.LogInfo($"[DebugInputSystem] Controls Changed for player {input.playerIndex}. " +
                          $"Current scheme: {input.currentControlScheme}");
            }
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
            if (action == null)
            {
                Log.LogWarning($"[DebugInputSystem] Action '{actionName}' not found in current action map.");
                return default;
            }

            return action.ReadValue<T>();
        }

        public bool IsActionPerformed(string actionName)
        {
            if (_playerInput == null)
                return false;

            var action = _playerInput.currentActionMap?.FindAction(actionName);
            if (action == null)
            {
                Log.LogWarning($"[DebugInputSystem] Action '{actionName}' not found in current action map.");
                return false;
            }

            return action.phase == InputActionPhase.Performed;
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

        private void OnGUI()
        {
            if (!showVisualFeedback || _playerInput == null)
                return;

            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = 20;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;

            GUI.Box(new Rect(10, 10, 300, 80),
                $"Action Map: {currentActionMap ?? "None"}\n" +
                $"Controls: {currentControlScheme ?? "None"}\n" +
                $"Listeners: {_onActionTriggered.listenerCount}", style);
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}
