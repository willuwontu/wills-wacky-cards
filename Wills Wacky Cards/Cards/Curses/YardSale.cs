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
using WWC.Extensions;
using UnboundLib.GameModes;
using System.Collections;
using HarmonyLib;

namespace WWC.Cards.Curses
{
    class YardSale : CustomCard, ICurseCard, IConditionalCard
    {
        private static CardInfo card;
        public CardInfo Card { get => card; set { if (!card) { card = value; } } }
        public bool Condition(Player player, CardInfo card)
        {
            if (card != YardSale.card)
            {
                return true;
            }

            if (!player || !player.data || !player.data.block || (player.data.currentCards == null))
            {
                return true;
            }

            if (player.data.currentCards.Contains(YardSale.card)) 
            {
                return false;
            }

            Dictionary<CardInfo, int> allowedCards = player.data.currentCards.ToDictionary(c => c, c => 0);

            Player[] enemies = PlayerManager.instance.players.Where(person => person.teamID != player.teamID).ToArray();

            foreach (var enemy in enemies)
            {
                foreach (var kvp in allowedCards) 
                {
                    if (ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(enemy, kvp.Key))
                    {
                        allowedCards[kvp.Key] += 1;
                    }
                }
            }

            if ((allowedCards.Count() < enemies.Length) || (allowedCards.Values.Where(c => c >= enemies.Length).Count() < enemies.Length))
            {
                return false;
            }

            return true;
        }

        public static bool yardSaleActive = false;

        internal static Player yardSalePlayer = null;

        internal static bool generatingCardsDuringYardSale = false;

        private static Dictionary<int, List<CardInfo>> cardsToRemove = new Dictionary<int, List<CardInfo>>();

        //public static bool YardSaleCondition(Player player, CardInfo card)
        //{
        //    if (!yardSaleActive)
        //    {
        //        return true;
        //    }

        //    if (!player || !card || !player.data || player.data.currentCards == null)
        //    {
        //        return true;
        //    }

        //    if (!player.data.currentCards.Contains(card))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseCategory, RerollManager.instance.NoFlip };
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            GameModeManager.AddOnceHook(GameModeHooks.HookPickEnd, StartYardSale, 1000);

            IEnumerator StartYardSale(IGameModeHandler gm)
            {
                yield return DoYardSale(player);
            }

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} added to Player {player.playerID}");
        }

        private static IEnumerator DoYardSale(Player player)
        {
            UnityEngine.Debug.Log("Starting Yard Sale");
            yardSaleActive = true;
            yardSalePlayer = player;

            Player[] enemies = PlayerManager.instance.players.Where(person => person.teamID != player.teamID).ToArray();
            
            foreach (var enemy in enemies) 
            {
                int currentPicksAllowed = DrawNCards.DrawNCards.GetPickerDraws(enemy.playerID);
                DrawNCards.DrawNCards.SetPickerDraws(enemy.playerID, player.data.currentCards.Where(card => ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(enemy, card)).Distinct().Count());
                
                for (int i = 0; i < 20; i++)
                {
                    yield return null;
                }
                yield return new WaitForSecondsRealtime(1f);
                yield return GameModeManager.TriggerHook(GameModeHooks.HookPlayerPickStart);
                CardChoiceVisuals.instance.Show(Enumerable.Range(0, PlayerManager.instance.players.Count).Where(i => PlayerManager.instance.players[i].playerID == enemy.playerID).First(), true);
                yield return CardChoice.instance.DoPick(1, enemy.playerID, PickerType.Player);
                yield return new WaitForSecondsRealtime(1f);

                for (int i = 0; i < 20; i++)
                {
                    yield return null;
                }

                if (cardsToRemove.TryGetValue(player.playerID, out var cards))
                {
                    ModdingUtils.Utils.Cards.instance.RemoveCardsFromPlayer(player, cards.ToArray(), ModdingUtils.Utils.Cards.SelectionType.Random);
                    cardsToRemove.Remove(player.playerID);
                }

                yield return new WaitForSecondsRealtime(1f);
                yield return GameModeManager.TriggerHook(GameModeHooks.HookPlayerPickEnd);
                yield return new WaitForSecondsRealtime(0.1f);

                DrawNCards.DrawNCards.SetPickerDraws(enemy.playerID, currentPicksAllowed);
            }

            if (cardsToRemove.TryGetValue(player.playerID, out var cards2))
            {
                cards2.Insert(0, YardSale.card);
                ModdingUtils.Utils.Cards.instance.RemoveCardsFromPlayer(player, cards2.ToArray(), ModdingUtils.Utils.Cards.SelectionType.Random);
                cardsToRemove.Remove(player.playerID);
            }

            yardSalePlayer = null;
            yardSaleActive = false;
            UnityEngine.Debug.Log("Exiting Yard Sale");
            yield break;
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Yard Sale";
        }
        protected override string GetDescription()
        {
            return "Everything's discounted, especially after sitting out in the rain.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return Rarities.Epic;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CurseManager.instance.FallenPurple;
        }
        public override string GetModName()
        {
            return WillsWackyCards.CurseInitials;
        }
        public override bool GetEnabled()
        {
            return true;
        }

        [UnboundLib.Networking.UnboundRPC]
        public static void URPCA_AddToListOfCardsToRemove(int playerID, string cardName)
        {
            //UnityEngine.Debug.Log($"{cardName} flagged to be removed for player {playerID}.");
            if (PlayerManager.instance.GetPlayerWithID(playerID) != null) 
            {
                //UnityEngine.Debug.Log($"Fetching card on player {playerID}.");
                CardInfo card = PlayerManager.instance.GetPlayerWithID(playerID).data.currentCards.FirstOrDefault(c => c.name == cardName);

                //UnityEngine.Debug.Log($"Player {playerID} was found to have {card}.");
                if (card != null)
                {
                    if (cardsToRemove.TryGetValue(playerID, out var cards))
                    {
                        cards.Add(card);
                    }
                    else
                    {
                        cardsToRemove.Add(playerID, new List<CardInfo>() { card });
                    }
                }
                else
                {
                    //UnityEngine.Debug.Log($"The card was null!");
                }
            }
        }
    }

    [HarmonyPatch(typeof(ModdingUtils.Patches.CardChoicePatchGetRanomCard))]
    public class PickableHiddenCardsPatch
    {
        public static CardInfo[] SpawnedCards { 
            get {
                List<GameObject> cardObjs = (List<GameObject>)Traverse.Create(CardChoice.instance).Field("spawnedCards").GetValue();

                CardInfo[] cards = new CardInfo[cardObjs.Count];

                if (cards.Length > 0) 
                {
                    cards = cardObjs.Select(c => c.GetComponent<CardInfo>()).ToArray();
                }

                return cards;
            } 
        }

        public static bool executing = false;
        [HarmonyPrefix]
        [HarmonyBefore("root.rarity.lib", "pykess.rounds.plugins.deckcustomization")]
        [HarmonyPatch(nameof(ModdingUtils.Patches.CardChoicePatchGetRanomCard.OrignialGetRanomCard), typeof(CardInfo[]))]
        private static void YardSaleCardsOnly(ref CardInfo[] cards)
        {
            if (YardSale.yardSaleActive && YardSale.generatingCardsDuringYardSale && CardChoice.instance.pickrID >= 0 && !executing)
            {
                //UnityEngine.Debug.Log("Yard Sale is active, getting player performing yard sale.");
                if (YardSale.yardSalePlayer != null)
                {
                    //UnityEngine.Debug.Log($"Yard Sale player is player {YardSale.yardSalePlayer.playerID}, getting cards for sale.");

                    Player pickingPlayer = PlayerManager.instance.GetPlayerWithID(CardChoice.instance.pickrID);
                    CardInfo[] availableCards = YardSale.yardSalePlayer.data.currentCards.Distinct().ToArray();

                    //UnityEngine.Debug.Log($"Outputting starting list of available cards:");
                    //foreach (var card in availableCards) { UnityEngine.Debug.Log($"{card.cardName} : {card.name}"); }

                    if (availableCards.Length > 1)
                    {
                        availableCards = availableCards.Where(card => ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(pickingPlayer, card) && !SpawnedCards.Select(c => c.sourceCard.name).Contains(card.name)).Distinct().ToArray();
                    }
                    else
                    {
                        return;
                    }

                    UnityEngine.Debug.Log($"Outputting list of available cards:");
                    foreach (var card in availableCards) { UnityEngine.Debug.Log($"{card.cardName} : {card.name}"); }

                    //UnityEngine.Debug.Log("Redoing Pick.");
                    //executing = true;
                    cards = availableCards;
                    //__result = ModdingUtils.Patches.CardChoicePatchGetRanomCard.OrignialGetRanomCard(availableCards);
                    //executing = false;
                }
            }
        }
    }
}
