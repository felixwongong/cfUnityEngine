using System.Collections.Generic;
using cfEngine.Util;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class InitializationState : GameState
    {
        public override HashSet<GameStateId> Whitelist { get; } = new();
        public override GameStateId Id => GameStateId.Initialization;

        protected internal override void StartContext(GameStateMachine gsm, StateParam param)
        {
            Game.Pool.AddPool("Vfx", new PrefabPool<SpriteAnimation>(Game.Asset.Load<SpriteAnimation>("Vfx"), true));
        }
    }
}