﻿using cfEngine.Logging;

namespace cfUnityEngine.UI.UGUI
{
    public abstract partial class UIPanel: IUIPanel
    {
        public abstract string id { get; }
        public void Show()
        {
            Log.LogInfo("UIPanel.Show: {0}", id);
        }
        
        public void Hide()
        {
            Log.LogInfo("UIPanel.Hide: {0}", id);
        }
    }
}