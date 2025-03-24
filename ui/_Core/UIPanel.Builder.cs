using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace cfUnityEngine.UI
{
    public partial class UIPanel
    {
        public class Builder
        {
            private string _panelPath;
            private Task<TemplateContainer> loadTask = Task<TemplateContainer>.FromException<>();

            public Builder SetPath(string panelPath)
            {
                _panelPath = panelPath;
                return this;
            }

            public Builder LoadPanel()
            {
                
            }
            
            public class TemplateUninitializedException : Exception
            {
                public TemplateUninitializedException(Builder builder) : base()
                {
                    
                }

                private static string createMessage(Builder builder)
                {
                    if (string.IsNullOrEmpty(builder._panelPath))
                    {
                        return $"Panel load task uninitialized due to panelPath unset, call {nameof(SetPath)} first";
                    }

                    if (builder.loadTask == null)
                    {
                        return $"Panel load task uninitialized, call {nameof(LoadPanel)} first";
                    }
                }
            }
        }
    }
}