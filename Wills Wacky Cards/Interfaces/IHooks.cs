using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib.GameModes;

namespace WWC.Interfaces
{
    public class InterfaceGameModeHooksManager : MonoBehaviour
    {
        public static InterfaceGameModeHooksManager instance { get; private set; }

        private List<IGameStartHookHandler> gameStartHooks = new List<IGameStartHookHandler>();
        private List<IGameEndHookHandler> gameEndHooks = new List<IGameEndHookHandler>();
        private List<IPlayerPickEndHookHandler> playerPickEndHooks = new List<IPlayerPickEndHookHandler>();
        private List<IPlayerPickStartHookHandler> playerPickStartHooks = new List<IPlayerPickStartHookHandler>();
        private List<IPickStartHookHandler> pickStartHooks = new List<IPickStartHookHandler>();
        private List<IPickEndHookHandler> pickEndHooks = new List<IPickEndHookHandler>();
        private List<IPointStartHookHandler> pointStartHooks = new List<IPointStartHookHandler>();
        private List<IPointEndHookHandler> pointEndHooks = new List<IPointEndHookHandler>();
        private List<IRoundStartHookHandler> roundStartHooks = new List<IRoundStartHookHandler>();
        private List<IRoundEndHookHandler> roundEndHooks = new List<IRoundEndHookHandler>();
        private List<IBattleStartHookHandler> battleStartHooks = new List<IBattleStartHookHandler>();

        private void Start()
        {
            if (instance != null)
            {
                UnityEngine.GameObject.DestroyImmediate(this);
                return;
            }
            instance = this;

            GameModeManager.AddHook(GameModeHooks.HookGameEnd, GameEnd);
            GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);
            GameModeManager.AddHook(GameModeHooks.HookBattleStart, BattleStart);
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, PlayerPickStart);
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickEnd, PlayerPickEnd);
            GameModeManager.AddHook(GameModeHooks.HookPointStart, PointStart);
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, PointEnd);
            GameModeManager.AddHook(GameModeHooks.HookPickStart, PickStart);
            GameModeManager.AddHook(GameModeHooks.HookPickEnd, PickEnd);
            GameModeManager.AddHook(GameModeHooks.HookRoundStart, RoundStart);
            GameModeManager.AddHook(GameModeHooks.HookRoundEnd, RoundEnd);
        }

        public void RegisterHooks(object obj)
        {
            if (obj is IGameEndHookHandler gameEnd)
            {
                gameEndHooks.Add(gameEnd);
            }
            if (obj is IGameStartHookHandler gameStart)
            {
                gameStartHooks.Add(gameStart);
            }
            if (obj is IRoundEndHookHandler roundEnd)
            {
                roundEndHooks.Add(roundEnd);
            }
            if (obj is IRoundStartHookHandler roundStart)
            {
                roundStartHooks.Add(roundStart);
            }
            if (obj is IPointStartHookHandler pointStart)
            {
                pointStartHooks.Add(pointStart);
            }
            if (obj is IPointEndHookHandler pointEnd)
            {
                pointEndHooks.Add(pointEnd);
            }
            if (obj is IBattleStartHookHandler battleStart)
            {
                battleStartHooks.Add(battleStart);
            }
            if (obj is IPickStartHookHandler pickStart)
            {
                pickStartHooks.Add(pickStart);
            }
            if (obj is IPickEndHookHandler pickEnd)
            {
                pickEndHooks.Add(pickEnd);
            }
            if (obj is IPlayerPickStartHookHandler playerPickStart)
            {
                playerPickStartHooks.Add(playerPickStart);
            }
            if (obj is IPlayerPickEndHookHandler playerPickEnd)
            {
                playerPickEndHooks.Add(playerPickEnd);
            }
        }

        public void RemoveHooks(object obj)
        {
            if (obj is IGameEndHookHandler gameEnd)
            {
                gameEndHooks.Remove(gameEnd);
            }
            if (obj is IGameStartHookHandler gameStart)
            {
                gameStartHooks.Remove(gameStart);
            }
            if (obj is IRoundEndHookHandler roundEnd)
            {
                roundEndHooks.Remove(roundEnd);
            }
            if (obj is IRoundStartHookHandler roundStart)
            {
                roundStartHooks.Remove(roundStart);
            }
            if (obj is IPointStartHookHandler pointStart)
            {
                pointStartHooks.Remove(pointStart);
            }
            if (obj is IPointEndHookHandler pointEnd)
            {
                pointEndHooks.Remove(pointEnd);
            }
            if (obj is IBattleStartHookHandler battleStart)
            {
                battleStartHooks.Remove(battleStart);
            }
            if (obj is IPickStartHookHandler pickStart)
            {
                pickStartHooks.Remove(pickStart);
            }
            if (obj is IPickEndHookHandler pickEnd)
            {
                pickEndHooks.Remove(pickEnd);
            }
            if (obj is IPlayerPickStartHookHandler playerPickStart)
            {
                playerPickStartHooks.Remove(playerPickStart);
            }
            if (obj is IPlayerPickEndHookHandler playerPickEnd)
            {
                playerPickEndHooks.Remove(playerPickEnd);
            }
        }

        private IEnumerator GameStart(IGameModeHandler gm)
        {
            foreach (var hook in gameStartHooks)
            {
                hook.OnGameStart();
            }

            yield break;
        }
        private IEnumerator GameEnd(IGameModeHandler gm)
        {
            foreach (var hook in gameEndHooks)
            {
                hook.OnGameEnd();
            }

            yield break;
        }
        private IEnumerator RoundStart(IGameModeHandler gm)
        {
            foreach (var hook in roundStartHooks)
            {
                hook.OnRoundStart();
            }

            yield break;
        }
        private IEnumerator RoundEnd(IGameModeHandler gm)
        {
            foreach (var hook in roundEndHooks)
            {
                hook.OnRoundEnd();
            }

            yield break;
        }
        private IEnumerator PointStart(IGameModeHandler gm)
        {
            foreach (var hook in pointStartHooks)
            {
                hook.OnPointStart();
            }

            yield break;
        }
        private IEnumerator PointEnd(IGameModeHandler gm)
        {
            foreach (var hook in pointEndHooks)
            {
                hook.OnPointEnd();
            }

            yield break;
        }
        private IEnumerator BattleStart(IGameModeHandler gm)
        {
            foreach (var hook in battleStartHooks)
            {
                hook.OnBattleStart();
            }

            yield break;
        }
        private IEnumerator PickStart(IGameModeHandler gm)
        {
            foreach (var hook in pickStartHooks)
            {
                hook.OnPickStart();
            }

            yield break;
        }
        private IEnumerator PickEnd(IGameModeHandler gm)
        {
            foreach (var hook in pickEndHooks)
            {
                hook.OnPickEnd();
            }

            yield break;
        }
        private IEnumerator PlayerPickStart(IGameModeHandler gm)
        {
            foreach (var hook in playerPickStartHooks)
            {
                hook.OnPlayerPickStart();
            }

            yield break;
        }
        private IEnumerator PlayerPickEnd(IGameModeHandler gm)
        {
            foreach (var hook in playerPickEndHooks)
            {
                hook.OnPlayerPickEnd();
            }

            yield break;
        }
    }

    interface IGameStartHookHandler
    {
        public abstract void OnGameStart();
    }
    interface IGameEndHookHandler
    {
        public abstract void OnGameEnd();
    }
    interface IPlayerPickStartHookHandler
    {
        public abstract void OnPlayerPickStart();
    }
    interface IPlayerPickEndHookHandler
    {
        public abstract void OnPlayerPickEnd();
    }
    interface IPointEndHookHandler
    {
        public abstract void OnPointEnd();
    }
    interface IPointStartHookHandler
    {
        public abstract void OnPointStart();
    }
    interface IRoundEndHookHandler
    {
        public abstract void OnRoundEnd();
    }
    interface IRoundStartHookHandler
    {
        public abstract void OnRoundStart();
    }
    interface IPickStartHookHandler
    {
        public abstract void OnPickStart();
    }
    interface IPickEndHookHandler
    {
        public abstract void OnPickEnd();
    }
    interface IBattleStartHookHandler
    {
        public abstract void OnBattleStart();
    }
}
