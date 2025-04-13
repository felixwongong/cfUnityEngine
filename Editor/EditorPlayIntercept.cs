using System;
using System.Collections.Generic;
using UnityEditor;

namespace cfUnityEngine.Editor
{
    [InitializeOnLoad]
    public class EditorPlayIntercept
    {
        private static EditorPlayIntercept _instance;
        public static EditorPlayIntercept instance => _instance ??= new EditorPlayIntercept();
        
        static EditorPlayIntercept()
        {
        }

        private readonly List<WeakReference<Action>> _playModeInterceptActions;

        private EditorPlayIntercept()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            _playModeInterceptActions = new();
        }
        
        ~EditorPlayIntercept()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        
        public void RegisterPlayModeIntercept(Action action)
        {
            if (action == null)
                return;

            _playModeInterceptActions.Add(new WeakReference<Action>(action));
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingEditMode)
                return;

            EditorApplication.isPlaying = false;
            _playModeInterceptActions.RemoveAll(wr => !wr.TryGetTarget(out _));
            
            foreach (var wr in _playModeInterceptActions)
            {
                if (wr.TryGetTarget(out var action))
                {
                    action?.Invoke();
                }
            }

            EditorApplication.isPlaying = true;
        }
    }
}