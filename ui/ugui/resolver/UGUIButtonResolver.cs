using System;
using cfEngine.Logging;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace cfUnityEngine
{
    [RequireComponent(typeof(Button))]
    public class UGUIButtonResolver : PropertyObjectResolver
    {
        [SerializeField] private Button _button;

        private void Awake()
        {
            Assert.IsNotNull(_button, "[UGUIButtonResolver] Button component is not assigned.");
        }

        protected override void OnResolve(object value)
        {
            if(value is Action action)
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(() => action.Invoke());
            }
            else
            {
                Log.LogError($"[UGUIButtonResolver] Property value is not an Action, propertyName: {propertyName}, type: {value.GetType()}");
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
        }
#endif
    }
}
