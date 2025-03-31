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

        protected INamespaceScope BindSubspace(INamespaceScope scope, string subNsName, IPropertySource source)
        {
            if (scope.@namespace.Equals(subNsName))
            {
                BindScopeBinders(scope);
                return scope;
            }

            var subscope = scope.GetSubspace(subNsName);
            if (subscope != null)
            {
                BindScopeBinders(subscope);
            }

            return subscope;

            void BindScopeBinders(INamespaceScope scope)
            {
                if (scope.TryGetScopeComponents<IPropertyBinder>(out var binders))
                {
                    foreach (var binder in binders.Span)
                    {
                        binder.BindSource(source);
                    }
                }
            }
        }
    }
}