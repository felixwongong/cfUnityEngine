using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace cfUnityEngine.Command
{
    public interface ICommand
    {
        public void Execute(IReadOnlyDictionary<string, string> args);

        public class HintAttribute : Attribute
        {
            public readonly string description;
            public HintAttribute(string description)
            {
                this.description = description;
            }
        }
    }
}