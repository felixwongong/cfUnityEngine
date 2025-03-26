using cfEngine.Asset;
using cfEngine.Core;
using cfEngine.Extension;
using cfEngine.Logging;
using cfEngine.Rx;
using cfEngine.Util;
using UnityEngine;
using UnityEngine.UIElements;

namespace cfUnityEngine.UI
{
    public class SpriteElement : UIElement<VisualElement>
    {
        public readonly Rt<string> spritePath = new();
        public readonly Rt<Sprite> sprite = new();

        private Subscription _spritePathSub;
        private Subscription _spriteSub;

        public override void Dispose()
        {
            base.Dispose();

            _spritePathSub.UnsubscribeIfNotNull();
            spritePath.Dispose();

            _spriteSub.UnsubscribeIfNotNull();
            sprite.Dispose();
        }

        protected override void OnVisualAttached()
        {
            _spritePathSub.UnsubscribeIfNotNull();
            SetSprite(spritePath);
            _spritePathSub = spritePath.Events.Subscribe(onUpdate: (_, newListItem) =>
            {
                SetSprite(newListItem.item);
            });

            _spriteSub.UnsubscribeIfNotNull();
            VisualElement.style.backgroundImage = new StyleBackground(sprite);
            _spriteSub = sprite.Events.Subscribe(onUpdate: (_, newListItem) =>
            {
                VisualElement.style.backgroundImage = new StyleBackground(newListItem.item);
            });
        }

        private void SetSprite(string spritePath)
        {
            if (!string.IsNullOrEmpty(spritePath))
            {
                Game.Current.GetAsset<Object>().LoadAsync<Sprite>(spritePath)
                    .ContinueWithSynchronized(t =>
                    {
                        if (!t.IsCompletedSuccessfully)
                        {
                            Log.LogException(t.Exception);
                            return;
                        }

                        sprite.Set(t.Result);
                    });
            }
        }
    }
}