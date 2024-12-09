#if CF_REACTIVE_DEBUG
using System;
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
        [SerializeField] private VisualTreeAsset _tabWindowAsset;

        private ListView subscriptionList;
        private List<WeakReference<Subscription>> currentCollectionSubs;

        [MenuItem("Cf Tools/Rt Collection Viewer")]
        public static void ShowPanel()
        {
            RtCollectionViewer wnd = GetWindow<RtCollectionViewer>();
            wnd.titleContent = new GUIContent(nameof(RtCollectionViewer));
        }

        private void CreateGUI()
        {
            var tabWindow = _tabWindowAsset.CloneTree();
            var tabList = tabWindow.Q<ListView>("tab-list");
            var tabContent = tabWindow.Q("tab-content");
            
            if (!EditorApplication.isPlaying)
            {
                rootVisualElement.Add(new Label("Editor is not in play mode"));
            }
            else
            {
                var collectionMap = _RtDebug.Instance.Collections;
                var collectionList = collectionMap.ToList();

                tabList.itemsSource = collectionList;
                tabList.makeItem = () => new Button();
                tabList.bindItem = (item, index) =>
                {
                    var button = (Button)item;
                    if (collectionList[index].Value.TryGetTarget(out var collection))
                    {
                        button.text = collection.GetType().GetTypeName();
                        button.clicked += () =>
                        {
                            if (collection is IDebugMarked debugSymbol)
                            {
                                var subMap = _RtDebug.Instance.GetCollectionSubs(debugSymbol.__GetId());
                                if (subMap != null)
                                {
                                    currentCollectionSubs = subMap.Values.ToList();
                                    subscriptionList.itemsSource = currentCollectionSubs;
                                }
                            }
                        };
                    }
                };

                subscriptionList = new ListView
                {
                    itemsSource = currentCollectionSubs,
                    makeItem = () => new Label(),
                    bindItem = (item, index) =>
                    {
                        var label = (Label)item;

                        if (currentCollectionSubs[index].TryGetTarget(out var subscription))
                        {
                            label.text = subscription.__GetDebugInfo();
                        }
                    }
                };
                tabContent.Add(subscriptionList);
            }
            
            rootVisualElement.Add(tabWindow);
        }
    }
}
#endif
