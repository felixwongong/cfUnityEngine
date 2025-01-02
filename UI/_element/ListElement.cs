using System.Collections.Generic;
using System.Linq;
using cfEngine.Rt;

namespace cfUnityEngine.UI
{
    public class ListElement<T> : UIElement<ReadOnlyListView> where T : UIElement
    {
        private Rt<IEnumerable<UIElement>> _itemsSource = new();

        private Subscription _onItemSourceUpdateSub;

        public override void Dispose()
        {
            base.Dispose();

            _onItemSourceUpdateSub.UnsubscribeIfNotNull();

            if (_itemsSource.Value != null)
            {
                foreach (var uiElements in _itemsSource.Value)
                {
                    uiElements.Dispose();
                }
            }

            _itemsSource.Dispose();
        }

        protected override void OnVisualAttached()
        {
            _onItemSourceUpdateSub.UnsubscribeIfNotNull();
            VisualElement.itemsSource = _itemsSource.Value;
            _onItemSourceUpdateSub = _itemsSource.Events.Subscribe(onUpdate: (oldItem, newItem) =>
            {
                foreach (var uiElement in oldItem.item)
                {
                    uiElement.Dispose();
                }

                VisualElement.itemsSource = newItem.item;
            });
        }

        public void SetItemsSource(RtReadOnlyList<T> items)
        {
            _itemsSource.Set(items.ToUISource());
        }

        public void SetItemsSource(IReadOnlyList<T> items)
        {
            _itemsSource.Set(items.Select(t => (UIElement)t));
        }
    }
}