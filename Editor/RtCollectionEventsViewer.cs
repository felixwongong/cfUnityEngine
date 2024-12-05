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
    public class RtCollectionEventsViewer: EditorWindow
    {
        [MenuItem("Cf Tools/Rt Collection Events Viewer")]
        public static void ShowPanel()
        {
            RtCollectionEventsViewer wnd = GetWindow<RtCollectionEventsViewer>();
            wnd.titleContent = new GUIContent(nameof(RtCollectionEventsViewer));
        }

        public class SubscriptionDisplay
        {
            public string displayName;
        }

        private void CreateGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                rootVisualElement.Add(new Label("Editor is not in play mode"));
            }
            else
            {
                var collectionMap = CollectionEventsBase.Debug.Instance.recorded;

                var scrollView = new ScrollView();
                foreach (var (collectionEvents, subscriptions) in collectionMap)
                {
                    var type = collectionEvents.GetType();
                    var eventName = new Label(type.Name);
                    var treeView = new TreeView();
                    var list = subscriptions.Select(kvp =>
                    {
                        if(kvp.Value.TryGetTarget(out var subscription))
                            return new TreeViewItemData<SubscriptionDisplay>(subscription.Id, new SubscriptionDisplay() {displayName = subscription.ToString()});
                        
                        return new TreeViewItemData<SubscriptionDisplay>();
                    }).ToList();
                   
                    treeView.SetRootItems(list);

                    treeView.makeItem = () => new Label();
                    treeView.bindItem = (item, index) =>
                    {
                        (item as Label).text = treeView.GetItemDataForIndex<SubscriptionDisplay>(index) is { } data ? data.displayName : "Empty";
                    };
                    
                    scrollView.Add(eventName);
                    scrollView.Add(treeView);
                }
                
                rootVisualElement.Add(scrollView);
            }
        }
    }
}