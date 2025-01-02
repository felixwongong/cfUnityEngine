using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace cfUnityEngine.UI
{
    public abstract class UIPanel : UIElement<TemplateContainer>
    {
        [Preserve]
        protected UIPanel()
        {
        }

        public void ShowPanel()
        {
            _ShowPanel();
        }

        protected virtual void _ShowPanel()
        {
            VisualElement.enabledSelf = true;
            VisualElement.RemoveFromClassList("hide");
            VisualElement.AddToClassList("show");
            OnPanelShown();
        }

        protected virtual void OnPanelShown()
        {
        }

        public virtual void HidePanel()
        {
            VisualElement.RemoveFromClassList("show");
            VisualElement.AddToClassList("hide");
            VisualElement.enabledSelf = false;
        }
    }
}