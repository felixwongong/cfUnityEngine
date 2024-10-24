using System.Collections.Generic;

namespace cfUnityEngine.Util
{
    public abstract class ActionCommand<TCommandType, TCommand> where TCommand : ActionCommand<TCommandType, TCommand>
    {
        public abstract TCommandType type { get; }
        private ExecutionContext _context;
        public ExecutionContext Context => _context;

        public class ExecutionContext
        {
            public ActionCommandController<TCommandType, TCommand> Controller;
            public float ExecutionTime;
            public IEnumerable<CommandPattern<TCommandType>> MatchedPatterns;
        }

        public bool TryExecute(in ExecutionContext context)
        {
            _context = context;
            return _Execute(context);
        }

        protected abstract bool _Execute(in ExecutionContext context);
    }
}