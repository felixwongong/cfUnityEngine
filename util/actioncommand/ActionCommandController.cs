using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace cfUnityEngine.Util
{
    public class ActionCommandController<TCommandType, TCommand>
    where TCommand: ActionCommand<TCommandType, TCommand>
    {
        private readonly int _commandBufferCount;
        private readonly List<TCommand> _commandQueue = new();

        private readonly Dictionary<TCommandType, List<CommandPattern<TCommandType>>> _commandPatternMap = new();

        public ActionCommandController(int commandBufferCount)
        {
            _commandBufferCount = commandBufferCount;
        }

        public void RegisterPattern(CommandPattern<TCommandType> pattern)
        {
            var patterns = TryGetCommandPatterns(pattern.commandType);
            patterns.Add(pattern);
        }

        public void ExecuteCommand(TCommand command)
        {
            if (_commandQueue.Count > _commandBufferCount)
            {
                _commandQueue.RemoveRange(_commandBufferCount - 2, _commandQueue.Count - (_commandBufferCount - 1));
            }

            var patterns = TryGetCommandPatterns(command.type);

            var success = command.TryExecute(new ActionCommand<TCommandType, TCommand>.ExecutionContext()
            {
                Controller = this,
                ExecutionTime = Time.time,
                MatchedPatterns = patterns.Where(p => p.IsMatch(command, _commandQueue))
            });

            if (success)
            {
                _commandQueue.Insert(0, command);
            }
        }

        private List<CommandPattern<TCommandType>> TryGetCommandPatterns(TCommandType commandType)
        {
            if (!_commandPatternMap.TryGetValue(commandType, out var patterns))
            {
                patterns = new List<CommandPattern<TCommandType>>();
                _commandPatternMap[commandType] = patterns;
            }

            return patterns;
        }
    }
}