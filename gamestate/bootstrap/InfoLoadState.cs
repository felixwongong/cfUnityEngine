using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cfEngine.Core.Layer;
using cfEngine.Util;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class InfoLoadState : GameState
    {
        public override HashSet<GameStateId> Whitelist { get; } = new() { GameStateId.Login };
        public override GameStateId Id => GameStateId.InfoLoad;

        protected internal override void StartContext(StateParam stateParam)
        {
            foreach (var info in InfoLayer.infos)
            {
                Game.Info.RegisterInfo(info);
            }

            var infoLoadTasks = Game.Info.InfoMap.Values.Select(info => info.LoadSerializedAsync(Game.TaskToken));
            Task.WhenAll(infoLoadTasks).ContinueWith(t =>
            {
                StateMachine.TryGoToState(GameStateId.Login, new LoginState.Param()
                {
                    Platform = LoginPlatform.Local,
                    Token = new LoginToken()
                });
            }, Game.TaskToken, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}