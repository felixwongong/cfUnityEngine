using System;
using System.Collections.Generic;
using cfUnityEngine.Command;
using UnityEditor;
using CommandService = cfUnityEngine.Command.CommandService;

namespace cfUnityEngine.GoogleDrive
{
    public partial class GDriveMirrorSetting
    {
        [CommandService.RegisterOnInitialized(nameof(Register))]
        [ICommand.Hint("Show Google Drive Mirror Setting")]
        public struct ShowCommand : ICommand
        {
            public static void Register() => CommandService.RegisterCommand(new ShowCommand(), "setting", "googledrive");
            
            public void Execute(IReadOnlyDictionary<string, string> args)
            {
                var setting = GetSetting();
                if (args.Count <= 0)
                {
                    Selection.activeObject = setting;
                }
                else
                {
                    if (args.TryGetValue("-r", out _))
                    {
                        setting.Refresh();
                    }
                }
            }
        }
    }
}