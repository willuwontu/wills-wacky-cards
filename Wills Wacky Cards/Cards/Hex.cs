using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnboundLib.Cards;
using UnityEngine;

namespace WillsWackyCards.Cards
{
    class Hex : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("CardManipulation") };
            UnityEngine.Debug.Log("[WWC][Card] Hex Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            if (WillsWackyCards.curses.Count > 0)
            {
                UnityEngine.Debug.Log($"[WWC][Hex] Player {player.teamID} Cursing Enemies");
                CardInfo curse;

                foreach (var item in PlayerManager.instance.players.Where(other => other.teamID != player.teamID).ToList())
                {
                    curse = WillsWackyCards.GetRandomCurse();
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(item, curse, false, "", 2f, 0, true);
                    UnityEngine.Debug.Log($"[WWC][Hex] Player {item.teamID} cursed with {curse.cardName}.");
                }
            }
        }
        public override void OnRemoveCard()
        {
            //Drives me crazy
        }

        protected override string GetTitle()
        {
            return "Hex";
        }
        protected override string GetDescription()
        {
            return "Double, double toil and trouble, fire burn and caldron bubble.";
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
                    stat = "Your Foes",
                    amount = "Curse",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
        }
        public override string GetModName()
        {
            return "WWC";
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
