using System.Collections.Generic;
using cfEngine.Util;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class UserDataLoadState : GameState
    {
        public override HashSet<GameStateId> Whitelist { get; } = new() { GameStateId.Initialization };

        public override GameStateId Id => GameStateId.UserDataLoad;

        protected internal override void StartContext(StateParam stateParam)
        {
            Game.UserData.LoadInitializeAsync(Game.TaskToken).ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    StateMachine.TryGoToState(GameStateId.Initialization);
                }
            });
        }
    }
}