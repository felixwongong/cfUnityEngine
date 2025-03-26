using System;
using cfEngine.Rx;
using UnityEngine.UIElements;

namespace cfUnityEngine.UI.UIToolkit
{
    public class ButtonElement: UIElement<Button>
    {
        Rt<Action> _onClickAction = new();

        public override void Dispose()
        {
            base.Dispose();

            if (_onClickAction.Value != null && VisualElement != null)
            {
                VisualElement.clicked -= _onClickAction.Value;
            }
        
            _onClickAction.Dispose();
        }

        protected override void OnVisualAttached()
        {
            if (_onClickAction.Value != null)
            {
                VisualElement.clicked += _onClickAction.Value;
            }
        
            _onClickAction.Events.Subscribe(onUpdate: (oldAction, newAction) =>
            {
                VisualElement.clicked -= oldAction.item;
                VisualElement.clicked += newAction.item;
            });
        }
    
        public void SetOnClick(Action onClick)
        {
            _onClickAction.Set(onClick);
        }
    }
}