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
        [SerializeField] private VisualTreeAsset _visualTreeAsset;

        private ListView subscriptionList;

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

            var tabList = visualTreeRoot.Q<ListView>("tab-list");
            var rootCollectionIds = _RtDebug.Instance.GetRootCollectionIds();
            tabList.itemsSource = rootCollectionIds;
            tabList.makeItem = () => new Button();
            tabList.bindItem = (e, i) =>
            {
                var button = (Button) e;
                var collectionId = (Guid)rootCollectionIds[i];
                if(_RtDebug.Instance.Collections.TryGetValue(collectionId, out var collectionRef))
                {
                    if (collectionRef.TryGetTarget(out var collection))
                    {
                        button.text = collection.__GetDebugTitle();
                        button.clickable.clicked += () =>
                        {
                            if (_RtDebug.Instance.TryGetMutatedReferences(collectionId, out var mutatedIds))
                            {
                                Debug.Log(string.Join(',', mutatedIds));
                            }
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
    }
}
#endif
