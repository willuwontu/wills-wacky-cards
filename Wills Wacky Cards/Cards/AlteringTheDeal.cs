using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using WillsWackyManagers.UnityTools;
using Nullmanager;

namespace WWC.Cards
{
    class AlteringTheDeal : CustomCard, IConditionalCard
    {
        internal static CardInfo card;
        public CardInfo Card { get => card; set { if (!card) { card = value; } } }
        public bool Condition(Player player, CardInfo card)
        {
            if (card != AlteringTheDeal.card)
            {
                return true;
            }

            if (!player)
            {
                return true;
            }

            if (PlayerManager.instance.players.Where(p=> p.teamID != player.teamID).Any(p=> DrawNCards.DrawNCards.GetPickerDraws(p.playerID) < 2))
            {
                return false;
            }

            return true;
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {

        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.AjustNulls(5*PlayerManager.instance.players.Where(p=>p.teamID != player.teamID).Count());
            foreach (Player opponent in PlayerManager.instance.players.Where(p => player.teamID != p.teamID)) 
            {
                DrawNCards.DrawNCards.SetPickerDraws(opponent.playerID, DrawNCards.DrawNCards.GetPickerDraws(opponent.playerID) - 1);
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            foreach (Player opponent in PlayerManager.instance.players.Where(p => player.teamID != p.teamID))
            {
                DrawNCards.DrawNCards.SetPickerDraws(opponent.playerID, DrawNCards.DrawNCards.GetPickerDraws(opponent.playerID) + 1);
            }
        }

        protected override string GetTitle()
        {
            return "Altering the Deal";
        }
        protected override string GetDescription()
        {
            return "Pray I do not alter it any further.";
        }
        protected override GameObject GetCardArt()
        {
            GameObject art;

            try
            {
                art = WillsWackyManagers.WillsWackyManagers.instance.WWMAssets.LoadAsset<GameObject>("C_AlteringTheDeal");
            }
            catch
            {
                art = null;
            }

            return art;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return Rarities.Exotic;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Hand Size for Foes",
                    amount = "-1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Nulls per Foe",
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