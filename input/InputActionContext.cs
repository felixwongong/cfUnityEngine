using System;
using cfEngine;
using cfEngine.Input;

namespace cfUnityEngine.Input
{
    public class InputActionContext : IInputActionContext
    {
        public string actionName { get; }
        private object value { get; }
        public IInputActionContext.Phase phase { get; }

        public InputActionContext(string actionName, object value, IInputActionContext.Phase phase)
        {
            this.actionName = actionName;
            this.value = value;
            this.phase = phase;
        }
        
        // Need optimize boxing
        public Res<T, Exception> GetValue<T>()
        {
            try
            {
                return (T)value;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}