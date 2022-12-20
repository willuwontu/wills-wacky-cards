using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib.GameModes;

namespace WWC.Interfaces
{
    /// <summary>
    /// <para>A class used to run the game mode hook interfaces.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/>.</para>
    /// </summary>
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
        /// <summary>
        /// <para>Registers any interface hooks on an object so that they're triggered when the hook is called.</para>
        /// </summary>
        /// <param name="obj">The object to register</param>
        /// <example>
        /// An example showing how to register a hook when a monobehaviour is created.
        /// <code>
        /// public void Start()
        ///     {
        ///         InterfaceGameModeHooksManager.RegisterHooks(this);
        ///     }
        /// </code>
        /// </example>
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

        /// <summary>
        /// <para>Deregisters any gamemode hooks on an object, so that they're not triggered anymore.</para>
        /// </summary>
        /// <param name="obj">The object to deregister.</param>
        /// <example>
        /// An example showing how to remove a hook when a monobehaviour is destroyed.
        /// <code>
        /// public void OnDestroy()
        ///     {
        ///         InterfaceGameModeHooksManager.RemoveHooks(this);
        ///     }
        /// </code>
        /// </example>
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
                try
                {
                    hook.OnGameStart();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
        private IEnumerator GameEnd(IGameModeHandler gm)
        {
            foreach (var hook in gameEndHooks)
            {
                try
                {
                    hook.OnGameEnd();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
        private IEnumerator RoundStart(IGameModeHandler gm)
        {
            foreach (var hook in roundStartHooks)
            {
                try
                {
                    hook.OnRoundStart();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
        private IEnumerator RoundEnd(IGameModeHandler gm)
        {
            foreach (var hook in roundEndHooks)
            {
                try
                {
                    hook.OnRoundEnd();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
        private IEnumerator PointStart(IGameModeHandler gm)
        {
            foreach (var hook in pointStartHooks)
            {
                try
                {
                    hook.OnPointStart();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
        private IEnumerator PointEnd(IGameModeHandler gm)
        {
            foreach (var hook in pointEndHooks)
            {
                try
                {
                    hook.OnPointEnd();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
        private IEnumerator BattleStart(IGameModeHandler gm)
        {
            foreach (var hook in battleStartHooks)
            {
                try
                {
                    hook.OnBattleStart();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
        private IEnumerator PickStart(IGameModeHandler gm)
        {
            foreach (var hook in pickStartHooks)
            {
                try
                {
                    hook.OnPickStart();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
        private IEnumerator PickEnd(IGameModeHandler gm)
        {
            foreach (var hook in pickEndHooks)
            {
                try
                {
                    hook.OnPickEnd();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
        private IEnumerator PlayerPickStart(IGameModeHandler gm)
        {
            foreach (var hook in playerPickStartHooks)
            {
                try
                {
                    hook.OnPlayerPickStart();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
        private IEnumerator PlayerPickEnd(IGameModeHandler gm)
        {
            foreach (var hook in playerPickEndHooks)
            {
                try
                {
                    hook.OnPlayerPickEnd();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            yield break;
        }
    }

    /// <summary>
    /// <para>An interface for a Game Start gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>  
    /// </summary>
    public interface IGameStartHookHandler
    {
        void OnGameStart();
    }
    /// <summary>
    /// <para>An interface for a Game End gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>  
    /// </summary>
    public interface IGameEndHookHandler
    {
        void OnGameEnd();
    }
    /// <summary>
    /// <para>An interface for a Player Pick Start gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>   
    /// </summary>
    public interface IPlayerPickStartHookHandler
    {
        void OnPlayerPickStart();
    }
    /// <summary>
    /// <para>An interface for a Player Pick End gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>   
    /// </summary>
    public interface IPlayerPickEndHookHandler
    {
        void OnPlayerPickEnd();
    }
    /// <summary>
    /// <para>An interface for a Point End gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>   
    /// </summary>
    public interface IPointEndHookHandler
    {
        void OnPointEnd();
    }
    /// <summary>
    /// <para>An interface for a Point Start gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>   
    /// </summary>
    public interface IPointStartHookHandler
    {
        void OnPointStart();
    }
    /// <summary>
    /// <para>An interface for a Round End gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>   
    /// </summary>
    public interface IRoundEndHookHandler
    {
        void OnRoundEnd();
    }
    /// <summary>
    /// <para>An interface for a Round Start gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>   
    /// </summary>
    public interface IRoundStartHookHandler
    {
        void OnRoundStart();
    }
    /// <summary>
    /// <para>An interface for a Pick Start gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>   
    /// </summary>
    public interface IPickStartHookHandler
    {
        void OnPickStart();
    }
    /// <summary>
    /// <para>An interface for a Pick End gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>   
    /// </summary>
    public interface IPickEndHookHandler
    {
        void OnPickEnd();
    }
    /// <summary>
    /// <para>An interface for a Battle Start gamemode hook.</para>
    /// 
    /// <para>Make sure to register your hooks with <see cref="InterfaceGameModeHooksManager.RegisterHooks(object)"/> in <see cref="MonoBehaviour.Start()"/> and remove them with <see cref="InterfaceGameModeHooksManager.RemoveHooks(object)"/> in <see cref="MonoBehaviour.OnDestroy()"/>.</para>   
    /// </summary>
    public interface IBattleStartHookHandler
    {
        void OnBattleStart();
    }
}