using cfEngine.Service;

namespace cfUnityEngine.UI
{
    public class UIService: IService
    {
        private IUIRoot _root;
        public IUIRoot root => _root;
        
        public UIService(IUIRoot root)
        {
            _root = root;
        }
        
        public void Dispose()
        {
            _root.Dispose();
        }
    }
}