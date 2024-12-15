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
        }
    }
}
#endif
