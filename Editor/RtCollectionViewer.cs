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
                    }
                    else
                    {
                        collectionList.RemoveAt(index);
                    }
                };
            }
            
            rootVisualElement.Add(tabWindow);
        }
    }
}