using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using WillsWackyManagers.Utils;
using UnboundLib.Cards;
using UnboundLib.Utils;
using UnityEngine;
using ModdingUtils.Extensions;

namespace WWC.Cards
{
    class Hex : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            //cardInfo.categories = new CardCategory[] { CardChoiceSpawnUniqueCardPatch.CustomCategories.CustomCardCategories.instance.CardCategory("CardManipulation") };
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            if (CurseManager.instance.GetRaw().Count() > 0)
            {
                var curses = CurseManager.instance.GetRaw().Intersect(CardManager.cards.Values.ToArray().Where((card) => card.enabled).Select(card => card.cardInfo).ToArray()).ToList();
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Hex] Player {player.teamID} Cursing Enemies");

                foreach (var item in PlayerManager.instance.players.Where(other => other.teamID != player.teamID).ToList())
                {
                    CurseManager.instance.CursePlayer(item, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(item, curse); }); 
                }
            }
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
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
            return WillsWackyCards.ModInitials;
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
