using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.Pool;
using UnityEditor;

namespace cfEngine.Command
{
    static class CommandSearchWindow
    {
        [SearchItemProvider]
        public static SearchProvider CreateProvider()
        {
            return new SearchProvider("cmd", "Command")
            {
                filterId = "cmd:",
                priority = 1000,

                fetchItems = (context, items, provider) =>
                {
                    var words = context.searchQuery.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    using var commandHandle = ListPool<string>.Get(out var commands);
                    using var argHandle = DictionaryPool<string, string>.Get(out var args);
                    
                    ExtractCommandArgsFromWords(words, commands, args);
                    
                    var searchItems = new List<SearchItem>();
                    
                    FetchCommandsRecursive(
                        CommandService.instance,
                        commands,
                        new List<string>(),
                        searchItems,
                        context,
                        provider
                    );
                    
                    return searchItems;
                }
            };
        }
        
        private static void FetchCommandsRecursive(
            CommandService service,
            IReadOnlyList<string> query,
            List<string> currentPath,
            List<SearchItem> searchItems,
            SearchContext context,
            SearchProvider provider)
        {
            // Check handlers in current scope
            foreach (var (name, handler) in service.HandlerMap)
            {
                currentPath.Add(name.ToString());
                if (IsPartialMatchKeys(query, currentPath))
                {
                    // Create SearchItem for this handler
                    var commandPath = string.Join(" ", currentPath);
                    var item = provider.CreateItem(context, commandPath);
                    
                    string description = "Run command";
                    var hints = handler.GetType().GetCustomAttributes(typeof(ICommandHandler.HintAttribute), true);
                    if (hints.Length > 0)
                    {
                        if (hints[0] is ICommandHandler.HintAttribute hintAttribute)
                        {
                            description = hintAttribute.description;
                        }
                    }
                    item.description = description;
                    item.thumbnail = (Texture2D)EditorGUIUtility.IconContent("d_console.infoicon").image;
                    item.data = new CommandCall(context.searchQuery, handler);
                    searchItems.Add(item);
                }
                currentPath.RemoveAt(currentPath.Count - 1);
            }

            // Recurse into sub-scopes
            foreach (var (name, subService) in service.ServiceScopeMap)
            {
                currentPath.Add(name.ToString());
                if (IsPartialMatchKeys(query, currentPath))
                {
                    FetchCommandsRecursive(subService, query, currentPath, searchItems, context, provider);
                }
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }

        private static bool IsPartialMatchKeys(IReadOnlyList<string> commands, IReadOnlyList<string> keys)
        {
            if (commands.Count > keys.Count)
                return false;

            for (var i = 0; i < commands.Count; i++)
            {
                if (!keys[i].StartsWith(commands[i]))
                    return false;
            }

            return true;
        }

        public static void ExtractCommandArgsFromWords(string[] words, List<string> commands, Dictionary<string, string> args)
        {
            for (var startIndex = 0; startIndex < words.Length; startIndex++)
            {
                var word = words[startIndex];

                bool isArg = word.StartsWith("-");

                if (isArg)
                {
                    if (commands.Count == 0)
                    {
                        commands.AddRange(words.AsMemory()[..startIndex].ToArray());
                    }

                    int endIndex = startIndex + 1;
                    for (; endIndex < words.Length; endIndex++)
                    {
                        if (words[endIndex].StartsWith("-"))
                            break;
                    }

                    if (startIndex + 1 >= endIndex)
                    {
                        args[word] = string.Empty;
                    }
                    else
                    {
                        var arg = string.Join(' ', words.AsMemory()[(startIndex + 1)..endIndex].ToArray());
                        args[word] = arg;
                        startIndex = endIndex - 1;
                    }
                }
            }
            
            if (commands.Count == 0)
            {
                commands.AddRange(words);
            }
        }
    }

    public class CommandCall
    {
        public readonly string query;
        public readonly ICommandHandler command;

        public CommandCall(string query, ICommandHandler command)
        {
            this.query = query;
            this.command = command;
        }
    }

    static class CommandSearchActions
    {
        [SearchActionsProvider]
        public static IEnumerable<SearchAction> CommandHandlers()
        {
            yield return new SearchAction("cmd", "execute", null, "Execute Command")
            {
                handler = (item) =>
                {
                    if (item.data is CommandCall call)
                    {
                        CommandService.instance.Execute(call.query);
                    }
                }
            };
        }
    }
    
    public static class CommandSearchLauncher
    {
        private static ISearchView _searchView; 
        
        [MenuItem("Cf Tools/Command Search")]
        [Shortcut("Cf Tools/Search Command", KeyCode.Tab, ShortcutModifiers.Shift)]
        public static void OpenCommandSearch()
        {
            if (_searchView != null)
                _searchView.Close();
            
            _searchView = SearchService.ShowContextual("cmd", "Command", null, null, null, null, null, null);
        }
    }
}