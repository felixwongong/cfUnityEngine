using UnityEngine;

namespace cfUnityEngine.UI.UGUI
{
    [AddComponentMenu("cfUnityEngine/UI/UGUI/Resolver/ActiveInactiveResolver")]
    public class ActiveInactiveResolver: PropertyBoolResolver
    {
        [Editor.MethodButton]
        protected override void OnResolve(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}