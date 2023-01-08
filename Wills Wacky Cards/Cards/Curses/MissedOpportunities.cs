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
using WillsWackyManagers.UnityTools;
using Photon.Realtime;
using UnboundLib.Utils;

namespace WWC.Cards.Curses
{
    class MissedOpportunities : CustomCard, ICurseCard, IConditionalCard
    {
        private static CardInfo card;
        public CardInfo Card { get => card; set { if (!card) { card = value; } } }

        public static Dictionary<int, List<CardInfo>> cardsSeen = new Dictionary<int, List<CardInfo>>();

        internal Dictionary<int, bool> checkingCardsForPlayer = new Dictionary<int, bool>();

        public bool Condition(Player player, CardInfo card)
        {
            if (card == null) 
            { 
                return true; 
            }
            if (!player || !player.data || player.data.currentCards == null)
            {
                return true;
            }

            if (!(player.data.currentCards.Contains(MissedOpportunities.card)))
            {
                return true;
            }

            if (checkingCardsForPlayer.TryGetValue(player.playerID, out var flag))
            {
                if (flag)
                {
                    return true;
                }
            }

            if (!cardsSeen.ContainsKey(player.playerID))
            {
                cardsSeen[player.playerID] = new List<CardInfo>();
            }

            if (cardsSeen[player.playerID].Contains(card))
            {
                checkingCardsForPlayer[player.playerID] = true;

                bool havecardsAvailable = ModdingUtils.Utils.Cards.active.Any(c => ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, c) && !(cardsSeen[player.playerID].Contains(c)));

                //CardInfo[] availableCards = ModdingUtils.Utils.Cards.active.Where(c => ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, c)).ToArray();

                checkingCardsForPlayer[player.playerID] = false;

                if (havecardsAvailable) 
                {
                    //UnityEngine.Debug.Log($"Player {player.playerID} has cards available, skipping {card.cardName}.");

                    //foreach (var card2 in availableCards)
                    //{
                    //    UnityEngine.Debug.Log(card2.cardName);
                    //}

                    return false;
                }
                else
                {
                    //UnityEngine.Debug.Log($"Player {player.playerID} has no cards available, reseting the list of cards available.");
                    cardsSeen[player.playerID] = new List<CardInfo>();
                    return true;
                }
            }

            return true;
        }

        [UnboundLib.Networking.UnboundRPC]
        public static void URPCA_CardsSeen(int playerID, string[] cardNames)
        {
            foreach (var cardName in cardNames) 
            {
                CardInfo card = null;
                //UnityEngine.Debug.Log(cardName);
                try
                {
                    card = ModdingUtils.Utils.Cards.instance.GetCardWithObjectName(cardName);
                }
                catch (InvalidOperationException) { }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }

                if (card != null) 
                {
                    if (cardsSeen.ContainsKey(playerID)) 
                    {
                        cardsSeen[playerID].Add(card);
                    }
                    else
                    {
                        cardsSeen[playerID] = new List<CardInfo>();
                        cardsSeen[playerID].Add(card);
                    }
                }
            }
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseCategory };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Missed Opportunities";
        }
        protected override string GetDescription()
        {
            return "You only get one shot.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return Rarities.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {

            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CurseManager.instance.DefaultWhite;
        }
        public override string GetModName()
        {
            return WillsWackyCards.CurseInitials;
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
