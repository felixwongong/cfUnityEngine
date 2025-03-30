using System;
using cfUnityEngine.Editor;
using TMPro;
using UnityEngine;

namespace cfUnityEngine
{
    [AddComponentMenu("cfUnityEngine/UI/UGUI/Resolver/UGUITextResolver")]
    public class UGUITextResolver : PropertyObjectResolver
    {
        [SerializeField] private TextMeshProUGUI _text;
        
        [MethodButton]
        protected override void OnResolve(object value)
        {
            if (_text != null)
            {
                _text.text = value.ToString();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_text == null)
            {
                _text = GetComponent<TextMeshProUGUI>();
            }
        }
#endif
    }
}