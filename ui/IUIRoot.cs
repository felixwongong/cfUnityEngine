using System.Threading.Tasks;
using cfEngine.Asset;
using UnityEngine;

namespace cfUnityEngine.UI
{
    public interface IUIRoot
    {
        public void Initialize(AssetManager<Object> assetLoader);
        public T Register<T>(T panel, string path) where T : IUIPanel;

        public Task PreloadPanel(string panelId);

        public Task InstantiatePanel(string panelId);
    }
}