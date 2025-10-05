using UnityEditor;
using cfEngine.Command;
using CommandService = cfEngine.Command.CommandService;

namespace cfUnityEngine.GoogleDrive
{
    public partial class GDriveMirrorSetting
    {
        [InitializeOnLoad]
        private static class GDriveCommandRegistration
        {
            static GDriveCommandRegistration()
            {
                var googleDriveScope = new CommandService();
                CommandService.instance.RegisterScope("googledrive", googleDriveScope);
                googleDriveScope.RegisterHandler("setting", new ShowCommand());
            }
        }

        [ICommandHandler.Hint("Show Google Drive Mirror Setting")]
        public class ShowCommand : ICommandHandler
        {
            public void Execute(Parameters args)
            {
                var setting = GetSetting();
                if (args.Count == 0)
                {
                    Selection.activeObject = setting;
                }
            }
        }
    }
}
