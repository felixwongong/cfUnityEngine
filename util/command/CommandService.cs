using System.Collections.Generic;

namespace cfUnityEngine.Command
{
    public static class CommandService
    {
        public static Dictionary<string[], ICommand> commandMap = new()
        {
        };
        
        public static void RegisterCommand(ICommand resolver, params string[] commandKeys)
        {
            commandMap[commandKeys] = resolver;
        }
        
        public static void UnregisterCommand(params string[] command)
        {
            commandMap.Remove(command);
        }
    }
}