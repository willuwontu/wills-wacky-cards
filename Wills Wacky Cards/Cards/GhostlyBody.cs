using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WillsWackyManagers.Utils;
using WWC.Extensions;
using WWC.MonoBehaviours;
using WWC.Interfaces;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    class GhostlyBody : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseSpawnerCategory, CurseEater.CurseEaterClass, CustomCardCategories.instance.CardCategory("Ghost Body") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("Runic Wards") };

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(20, () => CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); }));
            player.gameObject.GetOrAddComponent<GhostBody_Mono>();
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Ghostly Body";
        }
        protected override string GetDescription()
        {
            return "As you gain more curses, your body begins to merge with the shadows and become resilient to damage. If you have 16 curses, you gain the power to fly.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Damage Reduction per curse",
                    amount = "+5%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Curse",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return WillsWackyCards.ModInitials;
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}

namespace WWC.MonoBehaviours
{
    [DisallowMultipleComponent]
    class GhostBody_Mono : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IGameStartHookHandler
    {
        private bool changedGravity = false;
        private float prevGravity;
        private float multiplier = 0f;
        private bool increased = false;
        private int curseCount = 0;
        private ModdingUtils.MonoBehaviours.InAirJumpEffect flight;

        private PlayerCollision col;

        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            col = player.GetComponent<PlayerCollision>();

            applyImmediately = false;
            this.SetLivesToEffect(int.MaxValue);
        }

        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName.ToLower() == "Ghostly Body".ToLower())
                {
                    haveCard = true;
                    break;
                }
            }

            if (!haveCard)
            {
                UnityEngine.GameObject.Destroy(this);
            }
        }

        public void OnPointStart()
        {
            CheckIfValid();
            if (!increased)
            {
                increased = true;
                var curses = CurseManager.instance.GetAllCursesOnPlayer(player).Count();
                curseCount = curses;
                multiplier = 0.05f * curses;
                stats.GetAdditionalData().DamageReduction += multiplier;

                gravityModifier.gravityForce_mult = (1f - (1f / 16f * (float)Mathf.Clamp(curseCount, 0, 16)));

                if (curseCount >= 20)
                {
                    col.enabled = false;
                }

                if (curseCount >= 16)
                {

                    flight = player.gameObject.AddComponent<ModdingUtils.MonoBehaviours.InAirJumpEffect>();
                    flight.SetJumpMult(0.1f);
                    flight.AddJumps(1000000);
                    flight.SetCostPerJump(1);
                    flight.SetContinuousTrigger(true);
                    flight.SetResetOnWallGrab(true);
                    flight.SetInterval(0.1f);
                }
            }

            ApplyModifiers();
        }

        public void OnPointEnd()
        {
            this.ClearModifiers();

            col.enabled = true;
            if (increased)
            {
                stats.GetAdditionalData().DamageReduction -= multiplier;

                if (curseCount >= 20)
                {
                    col.enabled = true;
                }

                if (curseCount >= 16)
                {
                    UnityEngine.GameObject.Destroy(flight);
                }

                increased = false;
            }
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnOnDestroy()
        {
            OnPointEnd();
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }
    }
}
