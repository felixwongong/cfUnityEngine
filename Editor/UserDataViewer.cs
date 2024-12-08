using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cfEngine.Rt;
using cfEngine.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace cfUnityEngine.Editor
{
    public class UserDataViewer: EditorWindow
    {
        [SerializeField] private VisualTreeAsset _userDataWindowAsset;

        [MenuItem("Cf Tools/UserData Viewer")]
        public static void ShowPanel()
        {
            UserDataViewer wnd = GetWindow<UserDataViewer>();
            wnd.titleContent = new GUIContent(nameof(UserDataViewer));
        }

        private void CreateGUI()
        {
            var userDataWindow = _userDataWindowAsset.CloneTree();
            var tabList = userDataWindow.Q<ListView>("tab-list");
            var tabContent = userDataWindow.Q("tab-content");

            if (EditorApplication.isPlaying)
            {
                var loadButton = userDataWindow.Q<Button>("load-button");
                var saveButton = userDataWindow.Q<Button>("save-button");

                saveButton.clicked += () =>
                {
                    Game.UserData.SaveAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            Debug.LogException(t.Exception);
                            return;
                        } 
                        Debug.Log("Save succeed");
                    });
                };
            }

            rootVisualElement.Add(userDataWindow);
        }
    }
}