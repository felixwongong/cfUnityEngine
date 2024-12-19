#if CF_REACTIVE_DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfEngine.Rt;
using cfEngine.Util;
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

            var rootCollectionIds = _RtDebug.Instance.GetRootCollectionIds();
            DrawCollectionTabs(rootCollectionIds);
        }

        private void RedrawCurrentCollection()
        {
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
            }
        }

        private void DrawCollectionTabs(IList collectionIds)
        {
            if (collectionIds is not { Count: > 0 })
            {
                _tabList.itemsSource = null;
                return;
            }
            
            _tabList.itemsSource = collectionIds;
            _tabList.makeItem = () => new Button();
            _tabList.bindItem = (e, i) =>
            {
                var button = (Button)e;
                var collectionId = (Guid)collectionIds[i];
                if (_RtDebug.Instance.Collections.TryGetValue(collectionId, out var collectionRef))
                {
                    if (collectionRef.TryGetTarget(out var collection))
                    {
                        button.text = collection.__GetDebugTitle();
                        button.clickable.clicked += () =>
                        {
                            _currentCollectionId = collectionId;

                            RedrawCurrentCollection();
                        };
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
