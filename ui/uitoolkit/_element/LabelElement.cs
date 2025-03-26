using System;
using System.Collections.Generic;
using System.Text;
using cfEngine.Logging;
using cfEngine.Rt;
using UnityEngine.UIElements;

namespace cfUnityEngine.UI.UIToolkit
{
    //TODO: need optimized for string template & string builder
    public class LabelElement : UIElement<Label>
    {
        private RtDictionary<string, string> _templateTextMap = new();
        private string templateString;

        private Subscription _templateMapSub;
        private Subscription _textSub;
        private Dictionary<string, Subscription> _templateTextSub;

        public LabelElement(string defaultValue = "") : base()
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                SetText(defaultValue);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _templateMapSub.UnsubscribeIfNotNull();
            _templateTextMap.Dispose();

            templateString = string.Empty;

            _textSub.UnsubscribeIfNotNull();

            if (_templateTextSub != null)
            {
                foreach (var subscriptionHandle in _templateTextSub.Values)
                {
                    subscriptionHandle.UnsubscribeIfNotNull();
                }

                _templateTextSub.Clear();
            }
        }

        protected override void OnVisualAttached()
        {
            templateString = VisualElement.text;

            UpdateFromTemplate();
            _templateMapSub.UnsubscribeIfNotNull();
            _templateMapSub = _templateTextMap.Events.OnChange(UpdateFromTemplate);

            void UpdateFromTemplate()
            {
                var sb = _templateTextMap.TryGetValue(string.Empty, out var fullText)
                    ? new StringBuilder(fullText)
                    : new StringBuilder(templateString);

                foreach (var (template, text) in _templateTextMap)
                {
                    if (string.IsNullOrEmpty(template))
                        continue;

                    sb.Replace($"{{{template}}}", text);
                }

                VisualElement.text = sb.ToString();
            }
        }

        public void SetText(string text)
        {
            _templateTextMap.Upsert(string.Empty, text);
        }

        public void SetTemplate(string templateKey, string value)
        {
            if (string.IsNullOrEmpty(templateKey))
            {
                Log.LogException(new ArgumentNullException(nameof(templateKey),
                    "LabelElement.SetTemplate: templateKey is null or empty"));
                return;
            }

            _templateTextMap.Upsert(templateKey, value);
        }

        public void SetText(RtReadOnlyList<string> text)
        {
            _textSub.UnsubscribeIfNotNull();
            SetText(string.Concat(text));
            _textSub = text.Events.OnChange(() => SetText(string.Concat(text)));
        }

        public void SetTemplate(string templateKey, RtReadOnlyList<string> text)
        {
            _templateTextSub ??= new();
            if (_templateTextSub.TryGetValue(templateKey, out var handle))
            {
                handle.UnsubscribeIfNotNull();
            }

            SetTemplate(templateKey, string.Concat(text));
            _templateTextSub[templateKey] = text.Events
                .OnChange(() => { SetTemplate(templateKey, string.Concat(text)); });
        }
    }
}