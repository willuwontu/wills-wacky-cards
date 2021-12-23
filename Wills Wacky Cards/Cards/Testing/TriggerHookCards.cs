using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.GameModes;
using UnboundLib.Cards;
using WWC.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards.Testing
{
    class SimulateRoundEnd : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40,
                () => WillsWackyCards.instance.StartCoroutine(ISimulateRoundEnd())
                );
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        private IEnumerator ISimulateRoundEnd()
        {
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPointEnd);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookRoundEnd);
            yield break;
        }
        protected override string GetTitle()
        {
            return "Simulate Round End";
        }
        protected override string GetDescription()
        {
            return "Runs PointEnd, RoundEnd hooks.";
        }
    }
    class SimulateRoundStart : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40,
                () => WillsWackyCards.instance.StartCoroutine(ISimulateRoundStart())
                );
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        private IEnumerator ISimulateRoundStart()
        {
            yield return GameModeManager.TriggerHook(GameModeHooks.HookRoundStart);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPointStart);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookBattleStart);
            yield break;
        }
        protected override string GetTitle()
        {
            return "Simulate Round Start";
        }
        protected override string GetDescription()
        {
            return "Runs RoundStart, PointStart, BattleStart hooks.";
        }
    }
    class SimulatePickPhase : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, 
                () => WillsWackyCards.instance.StartCoroutine(ISimulatePick())
                );
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        private IEnumerator ISimulatePick()
        {
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPickStart);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPlayerPickStart);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPlayerPickEnd);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPlayerPickStart);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPlayerPickEnd);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPickEnd);
            yield break;
        }
        protected override string GetTitle()
        {
            return "Simulate Pick Phase";
        }
        protected override string GetDescription()
        {
            return "Runs PickStart, PlayerPickStart, PlayerPickEnd, PlayerPickStart, PlayerPickEnd, PickEnd hooks.";
        }
    }
    class SimulateRound : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40,
                () => WillsWackyCards.instance.StartCoroutine(ISimulateRound())
                );
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        private IEnumerator ISimulateRound()
        {
            yield return GameModeManager.TriggerHook(GameModeHooks.HookRoundStart);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPointStart);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookBattleStart);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPointEnd);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPointStart);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookBattleStart);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookPointEnd);
            yield return GameModeManager.TriggerHook(GameModeHooks.HookRoundEnd);
            yield break;
        }
        protected override string GetTitle()
        {
            return "Simulate Round";
        }
        protected override string GetDescription()
        {
            return "Runs RoundStart, PointStart, BattleStart, PointEnd, PointStart, BattleStart, PointEnd, RoundEnd hooks.";
        }
    }
    class DoInitStart : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookInitStart));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do InitStart";
        }
        protected override string GetDescription()
        {
            return "Runs the InitStart hook.";
        }
    }
    class DoInitEnd : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookInitEnd));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do InitEnd";
        }
        protected override string GetDescription()
        {
            return "Runs the InitEnd hook.";
        }
    }
    class DoPlayerPickEnd : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookPlayerPickEnd));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do PlayerPickEnd";
        }
        protected override string GetDescription()
        {
            return "Runs the PlayerPickEnd hook.";
        }
    }
    class DoPlayerPickStart : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookPlayerPickStart));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do PlayerPickStart";
        }
        protected override string GetDescription()
        {
            return "Runs the PlayerPickStart hook.";
        }
    }
    class DoPickEnd : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookPickEnd));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do PickEnd";
        }
        protected override string GetDescription()
        {
            return "Runs the PickEnd hook.";
        }
    }
    class DoPickStart : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookPickStart));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do PickStart";
        }
        protected override string GetDescription()
        {
            return "Runs the PickStart hook.";
        }
    }
    class DoBattleStart : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookBattleStart));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do BattleStart";
        }
        protected override string GetDescription()
        {
            return "Runs the BattleStart hook.";
        }
    }
    class DoPointEnd : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookPointEnd));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do PointEnd";
        }
        protected override string GetDescription()
        {
            return "Runs the PointEnd hook.";
        }
    }
    class DoPointStart : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookPointStart));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do PointStart";
        }
        protected override string GetDescription()
        {
            return "Runs the PointStart hook.";
        }
    }
    class DoRoundStart : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookRoundStart));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do RoundStart";
        }
        protected override string GetDescription()
        {
            return "Runs the RoundStart hook.";
        }
    }
    class DoRoundEnd : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookRoundEnd));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do RoundEnd";
        }
        protected override string GetDescription()
        {
            return "Runs the RoundEnd hook.";
        }
    }
    class DoGameEnd : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(GameModeHooks.HookGameEnd));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do GameEnd";
        }
        protected override string GetDescription()
        {
            return "Runs the GameEnd hook.";
        }
    }
    class DoGameStart : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => WillsWackyCards.instance.TriggerGameModeHook(UnboundLib.GameModes.GameModeHooks.HookGameStart));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Do GameStart";
        }
        protected override string GetDescription()
        {
            return "Runs the GameStart hook.";
        }
    }
}
