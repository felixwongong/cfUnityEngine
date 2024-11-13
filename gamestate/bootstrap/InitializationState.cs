using System.Collections.Generic;
using System.Threading.Tasks;
using cfEngine.Logging;
using cfEngine.Util;

namespace cfUnityEngine.GameState.Bootstrap
{
    public partial class InitializationState : GameState
    {
        public override HashSet<GameStateId> Whitelist { get; } = new() { GameStateId.BootstrapEnd };
        public override GameStateId Id => GameStateId.Initialization;

        protected internal override void StartContext(StateParam stateParam)
        {
            Initialize().ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    StateMachine.TryGoToState(GameStateId.BootstrapEnd);
                }
                else
                {
                    Log.LogException(t.Exception);
                }
            });
        }

        private partial Task Initialize();
    }
}
