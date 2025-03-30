using UnityEngine;

namespace cfUnityEngine.UI.UGUI
{
    public class ActiveInactiveResolver: PropertyBoolResolver
    {
        [Editor.MethodButton]
        protected override void OnResolve(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}