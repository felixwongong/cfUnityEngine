namespace cfUnityEngine.UI
{
    public interface IUIPanel
    {
        public string id { get; }
        public void Show();
        public void Hide();
        public void Bind(INamespaceScope scope);
    }
}