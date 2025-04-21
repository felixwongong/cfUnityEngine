namespace cfUnityEngine.UI.UGUI
{
    public abstract partial class UIPanel: PropertySource, IUIPanel
    {
        public abstract string id { get; }

        private bool enabled = false;
        
        public virtual void Show()
        {
            enabled = true;
            OnPropertyChanged(nameof(enabled), enabled);
        }

        public virtual void Hide()
        {
            enabled = false;
            OnPropertyChanged(nameof(enabled), enabled);
        }

        public virtual void Bind(INamespaceScope scope)
        {
            BindSubscope(scope, "this", this);
        }

        protected INamespaceScope BindSubscope(INamespaceScope scope, string childName, IPropertySource source)
        {
            if (scope.@namespace.Equals(childName))
            {
                scope.BindSource(source);
                return scope;
            }

            return scope.BindChildSource(childName, source);
        }
    }
}