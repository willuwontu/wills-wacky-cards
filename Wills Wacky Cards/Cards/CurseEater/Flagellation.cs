using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.MonoBehaviours;
using WillsWackyManagers.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using ClassesManagerReborn.Util;
using System.Collections;

namespace WWC.Cards
{
    class Flagellation : CustomClassCard
    {
        public override CardInfo Card { get => card; set { if (!card) { card = value; } } }
        public static CardInfo card = null;
        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = CurseEaterClass.name;
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseSpawnerCategory, CustomCardCategories.instance.CardCategory("cantEternity") };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.data.maxHealth *= 2f;
            player.data.health *= 2f;
            WillsWackyCards.instance.ExecuteAfterFrames(20, () => {
                WillsWackyCards.instance.StartCoroutine(GiveCurses(player, 5));
            });
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        public IEnumerator GiveCurses(Player player, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse, 3f); });
                for (int j = 0; j < 10; j++)
                {
                    yield return null;
                }
            }

            yield break;
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Drives me crazy
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }
        public override void OnReassignCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.data.maxHealth *= 2f;
            player.data.health *= 2f;
        }
        protected override string GetTitle()
        {
            return "Flagellation";
        }
        protected override string GetDescription()
        {
            return "Train your body to endure the horrors it contains.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "HP",
                    amount = "+100%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Curses",
                    amount = "+5",
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
