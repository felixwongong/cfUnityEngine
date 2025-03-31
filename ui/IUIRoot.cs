using System;
using System.Threading.Tasks;
using cfEngine.Asset;
using Object = UnityEngine.Object;

namespace cfUnityEngine.UI
{
    public interface IUIRoot: IDisposable
    {
        public void Initialize(AssetManager<Object> assetLoader);
        public T Register<T>(T panel, string path) where T : IUIPanel;

        public Task PreloadPanel(string panelId);

        public Task InstantiatePanel(string panelId);
        public IUIPanel GetPanel(string panelId);
        public T GetPanel<T>(string panelId) where T : IUIPanel;
    }

    public static class UIRoot
    {
        public static IUIRoot Current { get; private set; }
        public static void SetCurrent(IUIRoot instance)
        {
            Current = instance;
        }
    }
}