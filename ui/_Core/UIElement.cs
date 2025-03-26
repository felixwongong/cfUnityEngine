using System;
using cfEngine.Logging;
using cfEngine.Rx;
using UnityEngine.UIElements;

namespace cfUnityEngine.UI 
{
    public abstract class UIElementBase: IDisposable
    {
        public abstract void AttachFromRoot(VisualElement root, string visualElementName = null);
        public abstract void Dispose();
    }

    public abstract class UIElement<TVisualType>: UIElementBase, IDisposable where TVisualType: VisualElement
    {
        protected TVisualType VisualElement { get; private set; }
        public void AttachChild<T>(T uiElement, string childVisualElementName = null) where T: UIElementBase
        {
            if (VisualElement == null)
            {
                Log.LogException(new ArgumentNullException(nameof(VisualElement), "VisualElement is null"));
                return;
            }

            var element = uiElement;
            element.AttachFromRoot(VisualElement, childVisualElementName);
        }

        public override void AttachFromRoot(VisualElement root, string visualElementName = null)
        {
            AttachVisual(root.Q<TVisualType>(visualElementName));
        }

        public void AttachVisual(TVisualType visualElement)
        {
            if (visualElement == null)
            {
                return;
            }
            if (VisualElement != null)
            {
                VisualElement.dataSource = null;
            }
        
            VisualElement = visualElement;
            visualElement.dataSource = this;
        
            OnVisualAttached();
        }

        protected abstract void OnVisualAttached();

        public override void Dispose()
        {
            if (VisualElement != null)
            {
                VisualElement.dataSource = null;
            }
        }
    }

    public class UIElement : UIElement<VisualElement>
    {
        protected override void OnVisualAttached()
        {
        }
    }

    public static class UIElementExtension
    {
        public static RtReadOnlyList<UIElement> ToUISource<T>(this RtReadOnlyList<T> source) where T : UIElement
        {
            return source.select(t => (UIElement)t);
        }
    }
}