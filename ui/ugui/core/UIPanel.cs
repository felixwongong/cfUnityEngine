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
            BindSubspace(scope, "this", this);
        }

        protected INamespaceScope BindSubspace(INamespaceScope scope, string subscopeName, IPropertySource source)
        {
            if (scope.@namespace.Equals(subscopeName))
            {
                scope.SetBinderSource(source);
                return scope;
            }

            var subscope = scope.GetSubspace(subscopeName);
            subscope?.SetBinderSource(source);

            return subscope;
        }
    }
}