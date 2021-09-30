using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WillsWackyCards.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WillsWackyCards.Cards.Hidden
{
    class CounterfeitAmmo : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.ammo = -2;
            gun.reloadTimeAdd = 0.5f;
            gun.bursts = -1;
            gun.numberOfProjectiles = -1;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("Curse") };
            UnityEngine.Debug.Log($"[WWC][Curse] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[WWC][Curse] {GetTitle()} added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[WWC][Curse] {GetTitle()} removed from Player {player.playerID}");
        }
        protected override string GetTitle()
        {
            return "Counterfeit Ammo";
        }
        protected override string GetDescription()
        {
            return "Should've bought it from a reputable source.";
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
                    positive = false,
                    stat = "Reload Time",
                    amount = "+0.5s",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Ammo",
                    amount = "-2",
                    simepleAmount = CardInfoStat.SimpleAmount.lower
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Bullet",
                    amount = "-1",
                    simepleAmount = CardInfoStat.SimpleAmount.slightlyLower
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Burst",
                    amount = "-1",
                    simepleAmount = CardInfoStat.SimpleAmount.slightlyLower
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return "Curse";
        }
        public override bool GetEnabled()
        {
            return false;
        }
    }
}
