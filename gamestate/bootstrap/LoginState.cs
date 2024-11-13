using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfEngine.Logging;
using cfEngine.Util;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class LoginState : GameState
    {
        public override HashSet<GameStateId> Whitelist { get; } = new() { GameStateId.UserDataLoad };

        public class Param : StateParam
        {
            public LoginPlatform Platform;
            public LoginToken Token;
        }

        public override GameStateId Id => GameStateId.Login;

        protected internal override void StartContext(StateParam stateParam)
        {
            if (stateParam is not Param p)
            {
                var ex = new ArgumentNullException(nameof(stateParam), "Invalid param for Login State");
                Log.LogException(ex);
                throw ex;
            }

            LoginAsync(p).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    if (task.Result)
                    {
                        StateMachine.TryGoToState(GameStateId.UserDataLoad);
                    }
                    else
                    {
                        StateMachine.TryGoToState(GameStateId.Login, new Param()
                        {
                            Platform = LoginPlatform.Local,
                        });
                    }
                }
                else
                {
                    Log.LogException(task.Exception);
                }
            }, Game.TaskToken);
        }

        private async Task<bool> LoginAsync(Param param)
        {
            var token = Game.TaskToken;

            await Game.Auth.InitAsync(token);

            if (param.Platform == LoginPlatform.FromCached)
            {
                var loggedInCached = await Game.Auth.TryLoginCachedUserAsync(token);

                return loggedInCached;
            }

            if (!Game.Auth.IsSessionUserExist())
            {
                await Game.Auth.SignUpAsync(param.Platform, param.Token);
                return true;
            }
            else
            {
                //need to handle link account later
                return false;
            }
        }
    }
}