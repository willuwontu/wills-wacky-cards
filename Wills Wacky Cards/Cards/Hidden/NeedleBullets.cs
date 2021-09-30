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
    class NeedleBullets : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.projectileSize = 0.5f;
            gun.spread = 0.3f;
            gun.evenSpread = -0.1f;
            gun.reflects = -1;

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
            return "Needle Bullets";
        }
        protected override string GetDescription()
        {
            return "Hard to see, a pain to control, and likely to get lost in a haystack.";
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
                    stat = "Bullets",
                    amount = "Smaller",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Spread",
                    amount = "+30%",
                    simepleAmount = CardInfoStat.SimpleAmount.Some
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Bounces",
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
