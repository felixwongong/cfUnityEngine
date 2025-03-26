using System.Collections.Generic;
using System.Linq;
using cfEngine.Rx;
using cfEngine.Util;
using UnityEngine;
using UnityEngine.UIElements;

namespace cfUnityEngine.UI
{
    [UxmlElement]
    public partial class ReadOnlyListView : VisualElement
    {
        private VisualElement _contentContainer;
        public override VisualElement contentContainer => _contentContainer;

        [UxmlAttribute]
        public string contentContainerName
        {
            get => _contentContainer.name;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _contentContainer = this;
                    return;
                }

                var newContainer = this.Q(value);
                if (newContainer == null)
                {
                    Debug.LogError($"Content container not found. name: {value}");
                    return;
                }

                _contentContainer = newContainer;
            }
        }

        private VisualTreeAsset _itemTemplate;

        [UxmlAttribute]
        public VisualTreeAsset itemTemplate
        {
            get => _itemTemplate;
            set
            {
                _itemTemplate = value;
                Init();
            }
        }

        private IEnumerable<UIElement> _itemsSource;

        public IEnumerable<UIElement> itemsSource
        {
            get => _itemsSource;
            set
            {
                _itemsSource = value;
                Init();
            }
        }

        private readonly List<VisualElement> _itemElements = new();

#if CF_REACTIVE
        private Subscription _handle;
#endif

        public ReadOnlyListView()
        {
            _contentContainer = this;

            Init();
        }

        private void Init()
        {
            if (itemsSource == null || itemTemplate == null)
            {
                return;
            }

            _itemElements.Clear();

            _itemElements.AddRange(_contentContainer.Children());
            for (var i = 0; i < _itemsSource.Count(); i++)
            {
                SetDataSourceAtIndex(i);
            }

#if CF_REACTIVE
            if (itemsSource is RtReadOnlyList<UIElement> rtItemsSource)
            {
                _handle = rtItemsSource.Events.Subscribe(
                    onAdd: (indexedItem) => SetDataSourceAtIndex(indexedItem.index),
                    onRemove: (indexedItem) =>
                    {
                        indexedItem.item.Dispose();
                        var itemElement = _itemElements[indexedItem.index];
                        itemElement.dataSource = null;
                        _itemElements.RemoveAt(indexedItem.index);
                        itemElement.RemoveFromHierarchy();
                    },
                    onUpdate: (oldIndexed, newIndexed) =>
                    {
                        oldIndexed.item.Dispose();
                        var itemElement = _itemElements[newIndexed.index];
                        newIndexed.item.AttachVisual(itemElement);
                    }
                );
            }
#endif
        }

        private void SetDataSourceAtIndex(int index)
        {
            var itemSource = _itemsSource.ElementAt(index);

            VisualElement itemElement;

            if (index < _itemElements.Count)
            {
                itemElement = _itemElements[index];
            }
            else
            {
                itemElement = _itemTemplate.CloneTree();
                _contentContainer.Add(itemElement);
            }

            itemElement.dataSource = itemSource;
            itemSource.AttachVisual(itemElement);
        }

        public void _Clear()
        {
#if CF_REACTIVE
            _handle.UnsubscribeIfNotNull();
#endif

            foreach (var itemElement in _itemElements)
            {
                itemElement.dataSource = null;
            }

            _itemElements.Clear();
            _contentContainer.Clear();
        }
    }
}