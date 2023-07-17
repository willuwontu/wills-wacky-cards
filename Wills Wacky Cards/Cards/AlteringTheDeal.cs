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
using WillsWackyManagers.Utils;

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

            if (PlayerManager.instance.players.Where(p=> p.teamID != player.teamID).Any(p=> DrawNCards.DrawNCards.GetPickerDraws(p.playerID) > 1))
            {
                return false;
            }

            return true;
        }

        //public static Dictionary<Player, int> cardsTaken = new Dictionary<Player, int>();

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("cantEternity") };
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            foreach (Player opponent in PlayerManager.instance.players.Where(p => player.teamID != p.teamID)) 
            {
                var currentDraw = DrawNCards.DrawNCards.GetPickerDraws(opponent.playerID);

                if (currentDraw == 1)
                {
                    continue;
                }

                int drawRemoved = Mathf.CeilToInt(currentDraw * 0.75f);

                characterStats.AjustNulls(5 * drawRemoved);

                //if (AlteringTheDeal.cardsTaken.ContainsKey(opponent))
                //{
                //    AlteringTheDeal.cardsTaken[opponent] += currentDraw;
                //}
                //else
                //{
                //    AlteringTheDeal.cardsTaken[opponent] = currentDraw;
                //}

                DrawNCards.DrawNCards.SetPickerDraws(opponent.playerID, currentDraw - drawRemoved);
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //foreach (Player opponent in PlayerManager.instance.players.Where(p => player.teamID != p.teamID))
            //{
            //    DrawNCards.DrawNCards.SetPickerDraws(opponent.playerID, DrawNCards.DrawNCards.GetPickerDraws(opponent.playerID) + 1);
            //}
        }
        public override void OnReassignCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            foreach (Player opponent in PlayerManager.instance.players.Where(p => player.teamID != p.teamID))
            {
                var currentDraw = DrawNCards.DrawNCards.GetPickerDraws(opponent.playerID);

                if (currentDraw == 1)
                {
                    continue;
                }

                int drawRemoved = Mathf.CeilToInt(currentDraw * 0.75f);

                //if (AlteringTheDeal.cardsTaken.ContainsKey(opponent))
                //{
                //    AlteringTheDeal.cardsTaken[opponent] += currentDraw;
                //}
                //else
                //{
                //    AlteringTheDeal.cardsTaken[opponent] = currentDraw;
                //}

                DrawNCards.DrawNCards.SetPickerDraws(opponent.playerID, currentDraw - drawRemoved);
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
            return Rarities.Epic;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Hand Size for Foes",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Nulls per Card",
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