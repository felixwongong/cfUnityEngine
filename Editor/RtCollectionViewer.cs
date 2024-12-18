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
    public class RtCollectionViewer: EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;

        private ListView _tabList;
        private ListView _subscriptionList;
        private Label _currentCollectionLabel;

        private Guid currentCollectionId;

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
            var backButton = visualTreeRoot.Q<Button>("back-button");

            var rootCollectionIds = _RtDebug.Instance.GetRootCollectionIds();
            DrawCollectionTabs(rootCollectionIds);
        }

        private void RedrawCurrentCollection()
        {
            if (_RtDebug.Instance.Collections.TryGetValue(currentCollectionId, out var collectionRef) && collectionRef.TryGetTarget(out var currentCollection))
            {
                _currentCollectionLabel.text = currentCollection.__GetDebugTitle();
                
                if(_RtDebug.Instance.TryGetMutatedReferences(currentCollectionId, out var mutatedCollectionIds))
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
            _tabList.itemsSource = collectionIds;
            _tabList.makeItem = () => new Button();
            _tabList.bindItem = (e, i) =>
            {
                var button = (Button) e;
                var collectionId = (Guid)collectionIds[i];
                if(_RtDebug.Instance.Collections.TryGetValue(collectionId, out var collectionRef))
                {
                    if (collectionRef.TryGetTarget(out var collection))
                    {
                        button.text = collection.__GetDebugTitle();
                        button.clickable.clicked += () =>
                        {
                            currentCollectionId = collectionId;
                            
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

        private void UpdateBackButtonEnable()
        {
            if (currentCollectionId == Guid.Empty)
            {
            }
        }
    }
}
#endif
