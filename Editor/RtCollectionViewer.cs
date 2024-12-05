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
        
        [MenuItem("Cf Tools/Rt Collection Viewer")]
        public static void ShowPanel()
        {
            RtCollectionViewer wnd = GetWindow<RtCollectionViewer>();
            wnd.titleContent = new GUIContent(nameof(RtCollectionViewer));
        }

        public class SubscriptionDisplay
        {
            public string displayName;
        }

        private void CreateGUI()
        {
            var tabWindow = _tabWindowAsset.CloneTree();
            var tabList = tabWindow.Q<ListView>("tab-list");
            
            if (!EditorApplication.isPlaying)
            {
                rootVisualElement.Add(new Label("Editor is not in play mode"));
            }
            else
            {
                var collectionMap = CollectionEventsBase.Debug.Instance.RecordedCollectionSubscriptionId;
                var collectionList = collectionMap.Keys.ToList();

                tabList.itemsSource = collectionList;
                tabList.makeItem = () => new Button();
                tabList.bindItem = (item, index) =>
                {
                    var button = (Button)item;
                    button.text = collectionList[index];
                };
            }
            
            rootVisualElement.Add(tabWindow);
        }
    }
}