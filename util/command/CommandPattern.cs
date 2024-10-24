using System.Collections.Generic;

namespace cfUnityEngine.Util
{
    public abstract class CommandPattern<TCommandType>
    {
        public abstract TCommandType commandType { get; }
        public abstract bool IsMatch<TCommand>(TCommand newCommand, IReadOnlyList<TCommand> commandQueue)
            where TCommand: ActionCommand<TCommandType, TCommand>;
    }
}