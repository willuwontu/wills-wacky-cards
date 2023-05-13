using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using WWC.Interfaces;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using WillsWackyManagers.UnityTools;
using WWC.Cards.Curses;

namespace WWC.Cards
{
    class FutureSight : CustomCard, IConditionalCard
    {
        internal static CardInfo card;
        public CardInfo Card { get => card; set { if (!card) { card = value; } } }
        public bool Condition(Player player, CardInfo card)
        {
            if (card != FutureSight.card)
            {
                return true;
            }

            if (!player)
            {
                return true;
            }

            if (DrawNCards.DrawNCards.GetPickerDraws(player.playerID) > 25)
            {
                return false;
            }

            return true;
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = true;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            DrawNCards.DrawNCards.SetPickerDraws(player.playerID, DrawNCards.DrawNCards.GetPickerDraws(player.playerID) + 5);

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            DrawNCards.DrawNCards.SetPickerDraws(player.playerID, DrawNCards.DrawNCards.GetPickerDraws(player.playerID) - 5);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Future Sight";
        }
        protected override string GetDescription()
        {
            return "The possiblities are endless, but your power is not.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Hand Size",
                    amount = "+5",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Nulls per pick",
                    amount = "+10",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.MagicPink;
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
