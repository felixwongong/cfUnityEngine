using cfEngine.Util;
using cfUnityEngine.GameState.Bootstrap;

namespace cfUnityEngine.GameState
{
    public enum GameStateId
    {
        InfoLoad,
        Login,
        UserDataLoad,
        Initialization,
        BootstrapEnd
    }

    public abstract class GameState : State<GameStateId, GameState, GameStateMachine>
    {
    }

    public class GameStateMachine : StateMachine<GameStateId, GameState, GameStateMachine>
    {
        public GameStateMachine() : base()
        {
            RegisterState(new InfoLoadState());
            RegisterState(new LoginState());
            RegisterState(new UserDataLoadState());
            RegisterState(new InitializationState());
            RegisterState(new BootstrapEndState());
        }
    }
}