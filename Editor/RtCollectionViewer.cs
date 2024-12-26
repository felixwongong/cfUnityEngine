#if CF_REACTIVE_DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfEngine.Rt;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace cfUnityEngine.Editor
{
    public class RtCollectionViewer : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;

        private ListView _tabList;
        private ListView _subscriptionList;
        private ListView _contentList;
        private Label _currentCollectionLabel;

        private Guid _currentCollectionId;

        [MenuItem("Cf Tools/Rt Collection Viewer")]
        public static void ShowPanel()
        {
            RtCollectionViewer wnd = GetWindow<RtCollectionViewer>();
            wnd.titleContent = new GUIContent(nameof(RtCollectionViewer));
        }

        private void CreateGUI()
        {
            var visualTreeRoot = _visualTreeAsset.CloneTree();
            rootVisualElement.Add(visualTreeRoot);
            visualTreeRoot.StretchToParentSize();

            _tabList = visualTreeRoot.Q<ListView>("tab-list");
            _currentCollectionLabel = visualTreeRoot.Q<Label>("current-collection-label");
            _subscriptionList = visualTreeRoot.Q<ListView>("subscription-list");
            _contentList = visualTreeRoot.Q<ListView>("content-list");
            
            var backButton = visualTreeRoot.Q<Button>("back-button");
            backButton.clicked += () =>
            {
                if (!TryGetCollection(_currentCollectionId, out var currentCollection) || currentCollection.__GetSourceId() == Guid.Empty)
                {
                    _currentCollectionLabel.text = string.Empty;
                    _currentCollectionId = Guid.Empty;
                    DrawCollectionTabs(_RtDebug.Instance.GetRootCollectionIds());
                }
                else
                {
                    _currentCollectionId = currentCollection.__GetSourceId();
                    RedrawCurrentCollection();
                }
            };
            
            var disposeButton = visualTreeRoot.Q<Button>("dispose-button");
            disposeButton.clicked += ()　=>
            {
                if (TryGetCollection(_currentCollectionId, out var currentCollection))
                {
                    currentCollection.Dispose();
                    _currentCollectionId = currentCollection.__GetSourceId();
                    if (_currentCollectionId.Equals(Guid.Empty))
                    {
                        DrawCollectionTabs(_RtDebug.Instance.GetRootCollectionIds());
                    }
                    else
                    {
                        RedrawCurrentCollection();
                    }
                    
                    GC.Collect();
                }
            };

            var rootCollectionIds = _RtDebug.Instance.GetRootCollectionIds();
            DrawCollectionTabs(rootCollectionIds);
        }

        private void RedrawCurrentCollection()
        {
            GC.Collect();
            if (TryGetCollection(_currentCollectionId, out var currentCollection))
            {
                _currentCollectionLabel.text = currentCollection.__GetDebugTitle();

                if (_RtDebug.Instance.TryGetMutatedReferences(_currentCollectionId, out var mutatedCollectionIds))
                {
                    DrawCollectionTabs(mutatedCollectionIds);
                }
                else
                {
                    DrawCollectionTabs(new List<Guid>());
                }
                
                DrawSubscriptions(currentCollection);
                DrawContent(currentCollection);
            }
        }

        private void DrawCollectionTabs(IList collectionIds)
        {
            if (collectionIds is not { Count: > 0 })
            {
                _tabList.itemsSource = null;
                return;
            }

            var unsubscribeList = new List<Action>();

            _tabList.itemsSource = collectionIds;
            _tabList.makeItem = () => new Button();
            _tabList.bindItem = (e, i) =>
            {
                var button = (Button)e;
                var collectionId = (Guid)_tabList.itemsSource[i];
                if (_RtDebug.Instance.Collections.TryGetValue(collectionId, out var collectionRef))
                {
                    if (collectionRef.TryGetTarget(out var collection))
                    {
                        button.text = collection.__GetDebugTitle();
                        button.clickable.clicked += OnButtonClickedOnce;
                        unsubscribeList.Add(() => button.clickable.clicked -= OnButtonClickedOnce);

                        void OnButtonClickedOnce()
                        {
                            _currentCollectionId = collectionId;
                            RedrawCurrentCollection();
                        }
                    }
                    else
                    {
                        button.text = "DisposedCollection";
                    }
                }
                else
                {
                    button.text = "Collection not found";
                }
            };
            
            _tabList.unbindItem = (e, i) =>
            {
                foreach (var action in unsubscribeList)
                {
                    action?.Invoke();
                }
                unsubscribeList.Clear();
            };
        }
        
        private void DrawSubscriptions(ICollectionDebug collection)
        {
            if (_RtDebug.Instance.CollectionSubs.TryGetValue(collection.__GetId(), out var subscriptionMap))
            {
                var subscriptions = subscriptionMap.Values.ToList();
                _subscriptionList.itemsSource = subscriptions;
                _subscriptionList.makeItem = () => new Label();
                _subscriptionList.bindItem = (e, i) =>
                {
                    var label = (Label)e;
                    var subscriptionRef = (WeakReference<Subscription>)_subscriptionList.itemsSource[i];
                    if (subscriptionRef.TryGetTarget(out var subscription))
                    {
                        label.text = subscription.__GetDebugTitle();
                    }
                    else
                    {
                        label.text = "Disposed Subscription";
                    }
                };
            }
            else
            {
                _subscriptionList.itemsSource = null;
            }
        }
        
        private void DrawContent(ICollectionDebug collection)
        {
            if (collection is not IEnumerable ienumerable)
            {
                _contentList.itemsSource = null;
                return;
            }

            var contents = new List<object>();
            foreach (var x in ienumerable)
            {
                contents.Add(x);
            }

            _contentList.itemsSource = contents;
            _contentList.makeItem = () => new Label();
            _contentList.bindItem = (e, i) =>
            {
                var label = (Label)e;
                label.text = _contentList.itemsSource[i].ToString();
            };
        }

        private bool TryGetCollection(Guid collectionId, out ICollectionDebug collection)
        {
            if (_RtDebug.Instance.Collections.TryGetValue(collectionId, out var collectionRef))
            {
                if (collectionRef.TryGetTarget(out collection))
                {
                    return true;
                }
            }

            collection = null;
            return false;
        }
    }
}
#endif
