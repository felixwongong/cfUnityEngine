using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Search;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.Pool;

namespace cfUnityEngine.Command
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
                    var words = context.searchQuery.Split(' ');

                    using var commandHandle = ListPool<string>.Get(out var commands);
                    using var argHandle = DictionaryPool<string, string>.Get(out var args);
                    
                    ExtractCommandArgsFromWords(words, commands, args);
                    
                    var searchItems = new List<SearchItem>();
                    var sb = new StringBuilder();
                    foreach (var (keys, command) in CommandService.commandMap)
                    {
                        if(!IsPartialMatchKeys(commands, keys))
                            continue;

                        string description = $"Run command";;
                        var hints = command.GetType().GetCustomAttributes(typeof(ICommand.HintAttribute), true);
                        if (hints.Length > 0)
                        {
                            var hintAttribute = hints.FirstOrDefault() as ICommand.HintAttribute;
                            if (hintAttribute != null)
                            {
                                description = hintAttribute.description;
                            }
                        }
                        
                        var id = sb.AppendJoin(' ', keys).ToString();
                        var item = provider.CreateItem(context, id);
                        item.description = description;
                        item.thumbnail = (Texture2D)EditorGUIUtility.IconContent("d_console.infoicon").image;
                        item.data = new CommandCall(context.searchQuery, command);
                        searchItems.Add(item);

                        sb.Clear();
                    }
                    
                    return searchItems;
                }
            };
        }

        private static bool IsPartialMatchKeys(IReadOnlyList<string> commands, string[] keys)
        {
            if (commands.Count > keys.Length)
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
        public readonly ICommand command;

        public CommandCall(string query, ICommand command)
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
                        var words = call.query.Split(' ');
                        using var commandsHandle = ListPool<string>.Get(out var commands);
                        using var argsHandle = DictionaryPool<string, string>.Get(out var args);
                        CommandSearchWindow.ExtractCommandArgsFromWords(words, commands, args);

                        if (call.command != null)
                        {
                            call.command.Execute(args);
                        }
                        else
                        {
                            Debug.LogError($"Command not found: {string.Join(' ', commands)}");
                        }
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