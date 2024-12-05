using System;
using System.Collections.Generic;
using cfEngine.Rt;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace cfUnityEngine.Editor
{
    public class RtCollectionViewer: EditorWindow
    {
        [MenuItem("Cf Tools/Rt Collection Viewer")]
        public static void ShowPanel()
        {
            RtCollectionViewer wnd = GetWindow<RtCollectionViewer>();
            wnd.titleContent = new GUIContent(nameof(RtCollectionViewer));
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
                
                foreach (var (collectionEvents, subscriptions) in collectionMap)
                {
                    var treeView = new TreeView();
                }
            }
        }
    }
}