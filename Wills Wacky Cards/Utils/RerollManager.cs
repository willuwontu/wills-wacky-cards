using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnboundLib;
using UnboundLib.Utils;
using ModdingUtils.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

namespace WillsWackyCards.Utils
{
    /// <summary>
    /// Manages effects related to clearing out a player's cards and replacing all of them.
    /// </summary>
    public class RerollManager : MonoBehaviour
    {
        /// <summary>
        /// Instanced version of the class for accessibility from within static functions.
        /// </summary>
        public static RerollManager instance { get; private set; }

        /// <summary>
        /// The card category for cards that should not be given out after a table flip.
        /// </summary>
        public CardCategory NoFlip = CustomCardCategories.instance.CardCategory("NoFlip");
        
        /// <summary>
        /// The player responsible for the tableflip. Used to add the table flip card to the player.
        /// </summary>
        public Player flippingPlayer;

        /// <summary>
        /// When set to true, a table flip will be initiated at the next end of a player's pick. Initiate the FlipTable() method if you wish to flip before then.
        /// </summary>
        public bool tableFlipped;

        /// <summary>
        /// The table flip card itself. It's automatically given out to the flipping player aftr a table flip.
        /// </summary>
        public CardInfo tableFlipCard;

        /// <summary>
        /// A list of players to reroll when the next reroll is initiated.
        /// </summary>
        public List<Player> rerollPlayers = new List<Player>();

        /// <summary>
        /// When set to true, a reroll will be initiated at the next end of a player's pick. Initiate the Reroll() method if you wish to reroll before then.
        /// </summary>
        public bool reroll;

        private List<bool> rerolling = new List<bool>();

        /// <summary>
        /// The reroll card itself. It's automatically given out to the rerolling player after a table flip.
        /// </summary>
        public CardInfo rerollCard;

        private System.Random random = new System.Random();


        private void Start()
        {
            instance = this;
        }

        /// <summary>
        /// Initiates a table flip for all players.
        /// </summary>
        /// <param name="addCard">Whether the flipping player (if one exists) shoudl be given the Table Flip Card (if it exists).</param>
        /// <returns></returns>
        public IEnumerator FlipTable(bool addCard = true)
        {
            Dictionary<Player, List<Rarity>> cardRarities = new Dictionary<Player, List<Rarity>>();

            foreach (var player in PlayerManager.instance.players)
            {
                UnityEngine.Debug.Log($"[WWC][Debugging] Getting card rarities for player {player.playerID}");
                // Compile List of Rarities
                cardRarities.Add(player, player.data.currentCards.Select(card => CardRarity(card)).ToList());
                try
                {
                    ModdingUtils.Utils.Cards.instance.RemoveAllCardsFromPlayer(player);
                }
                catch (NullReferenceException)
                {
                    UnityEngine.Debug.Log($"[WWC][Debugging] SOMEBODY NEEDS TO FIX THEIR REMOVECARD FUNCTION.");
                    cardRarities[player].Clear();
                }
                UnityEngine.Debug.Log($"[WWC][Debugging] {cardRarities[player].Count} card rarities found for player {player.playerID}");
            }

            if (flippingPlayer && (flippingPlayer ? flippingPlayer.data.currentCards.Count : 0) > 0)
            {
                // Remove the last card from the flipping player, since it's going to be board wipe
                cardRarities[flippingPlayer].RemoveAt(cardRarities[flippingPlayer].Count - 1);
            }

            var allCards = CardManager.cards.Values.ToArray().Where(cardData => cardData.enabled && !(cardData.cardInfo.categories.Contains(NoFlip) || (cardData.cardInfo.cardName.ToLower() == "shuffle"))).Select(card => card.cardInfo).ToList();
            allCards.Remove(tableFlipCard);

            UnityEngine.Debug.Log($"[WWC][Debugging] {allCards.Count()} cards are enabled and ready to be swapped out.");

            yield return WaitFor.Frames(1);

            for (int i = 0; i < Mathf.Max(cardRarities.Values.Select(cards => cards.Count()).ToArray()); i++)
            {
                UnityEngine.Debug.Log($"[WWC][Debugging] Initiating round {i+1} of readding cards to players.");
                foreach (var player in PlayerManager.instance.players)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.RemoveAll(category => category == CurseManager.instance.curseCategory);
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseManager.instance.curseInteractionCategory);
                    if (CurseManager.instance.HasCurse(player))
                    {
                        ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.RemoveAll(category => category == CurseManager.instance.curseInteractionCategory);
                        UnityEngine.Debug.Log($"[WWC][Debugging] Player {player.playerID} is available for curse interaction effects");
                    }
                    else if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(CurseManager.instance.curseInteractionCategory))
                    {
                        ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseManager.instance.curseInteractionCategory);
                    }
                    UnityEngine.Debug.Log($"[WWC][Debugging] Checking player {player.playerID} to see if they are able to have a card added.");
                    if (i < cardRarities[player].Count)
                    {
                        UnityEngine.Debug.Log($"[WWC][Debugging] Player {player.playerID} is able to have a card added.");
                        var rarity = cardRarities[player][i];
                        UnityEngine.Debug.Log($"[WWC][Debugging] Player {player.playerID}'s card was originally {RarityName(rarity)}, finding a replacement now.");
                        var cardChoices = allCards.Where(cardInfo => (CardRarity(cardInfo) == rarity) && (ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, cardInfo))).ToArray();
                        UnityEngine.Debug.Log($"[WWC][Debugging] Player {player.playerID} is eligible for {cardChoices.Count()} cards");
                        if (cardChoices.Count() > 0)
                        {
                            var card = RandomCard(cardChoices);
                            UnityEngine.Debug.Log($"[WWC][Debugging] Player {player.playerID} is being given {card.cardName}");
                            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, false, "", 2f, 2f, true);
                            ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, card);
                        }
                    }
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseManager.instance.curseCategory);
                    yield return WaitFor.Frames(10);
                }

                yield return WaitFor.Frames(20); 
            }
            UnityEngine.Debug.Log($"[WWC][Debugging] Finished adding cards to players.");

            if (flippingPlayer && tableFlipCard && addCard)
            {
                // Add the tableflip card to the player
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(flippingPlayer, tableFlipCard, false, "", 2f, 2f, true);
                ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(flippingPlayer, tableFlipCard);
            }
            yield return WaitFor.Frames(20);

            flippingPlayer = null;
            tableFlipped = false;
            yield return null;
        }

        /// <summary>
        /// Initiates any rerolls in the queue.
        /// </summary>
        /// <param name="addCard">Whether a player should be given the Reroll card after they reroll.</param>
        /// <returns></returns>
        public IEnumerator InitiateRerolls(bool addCard = true)
        {
            foreach (var player in rerollPlayers)
            {
                StartCoroutine(Reroll(player));
            }

            yield return new WaitUntil(() => rerolling.Count <= 0);

            reroll = false;
            yield return null;
        }

        /// <summary>
        /// Rerolls any cards the specified player has.
        /// </summary>
        /// <param name="player">The player to reroll.</param>
        /// <param name="addCard">If true, the reroll card will be added to the player, if it exists.</param>
        /// <returns></returns>
        public IEnumerator Reroll(Player player, bool addCard = true)
        {
            rerolling.Add(true);
            if (player && (player ? player.data.currentCards.Count : 0) > 0)
            {
                List<Rarity> cardRarities = new List<Rarity>();

                UnityEngine.Debug.Log($"[WWC][Debugging] Getting card rarities for player {player.playerID}");
                cardRarities = player.data.currentCards.Select(card => CardRarity(card)).ToList();
                UnityEngine.Debug.Log($"[WWC][Debugging] {cardRarities.Count} card rarities found for player {player.playerID}");
                try
                {
                    ModdingUtils.Utils.Cards.instance.RemoveAllCardsFromPlayer(player);
                }
                catch (NullReferenceException)
                {
                    UnityEngine.Debug.Log($"[WWC][Debugging] SOMEBODY NEEDS TO FIX THEIR REMOVECARD FUNCTION.");
                    cardRarities.Clear();
                }

                if (cardRarities.Count > 0)
                {
                    // Remove the last card from the rerolling player, since it's going to be rerolling
                    cardRarities.RemoveAt(cardRarities.Count - 1); 
                }

                var allCards = CardManager.cards.Values.ToArray().Where(cardData => cardData.enabled && !(cardData.cardInfo.categories.Contains(NoFlip) || (cardData.cardInfo.cardName.ToLower() == "shuffle"))).Select(card => card.cardInfo).ToList();

                UnityEngine.Debug.Log($"[WWC][Debugging] {allCards.Count()} cards are enabled and ready to be swapped out.");

                yield return WaitFor.Frames(1);


                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.RemoveAll(category => category == CurseManager.instance.curseCategory);

                foreach (var rarity in cardRarities)
                {

                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseManager.instance.curseInteractionCategory);
                    if (CurseManager.instance.HasCurse(player))
                    {
                        ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.RemoveAll(category => category == CurseManager.instance.curseInteractionCategory);
                        UnityEngine.Debug.Log($"[WWC][Debugging] Player {player.playerID} is available for curse interaction effects");
                    }
                    else if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(CurseManager.instance.curseInteractionCategory))
                    {
                        ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseManager.instance.curseInteractionCategory);
                    }

                    UnityEngine.Debug.Log($"[WWC][Debugging] Checking player {player.playerID} to see if they are able to have a card added.");
                    UnityEngine.Debug.Log($"[WWC][Debugging] Player {player.playerID} is able to have a card added.");
                    UnityEngine.Debug.Log($"[WWC][Debugging] Player {player.playerID}'s card was originally {RarityName(rarity)}, finding a replacement now.");
                    var cardChoices = allCards.Where(cardInfo => (CardRarity(cardInfo) == rarity) && (ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, cardInfo))).ToArray();
                    UnityEngine.Debug.Log($"[WWC][Debugging] Player {player.playerID} is eligible for {cardChoices.Count()} cards");
                    if (cardChoices.Count() > 0)
                    {
                        var card = RandomCard(cardChoices);
                        UnityEngine.Debug.Log($"[WWC][Debugging] Player {player.playerID} is being given {card.cardName}");
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, false, "", 2f, 2f, true);
                        ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, card);
                    }

                    yield return WaitFor.Frames(20);
                }
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseManager.instance.curseCategory);
                UnityEngine.Debug.Log($"[WWC][Debugging] Finished adding cards.");

                if (rerollCard && addCard)
                {
                    // Add the tableflip card to the player
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, rerollCard, false, "", 2f, 2f, true);
                    ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, rerollCard);
                }
                yield return WaitFor.Frames(20);
            }
            rerolling.Remove(true);

            yield return null;
        }

        private enum Rarity
        {
            Rare,
            Uncommon,
            Common,
            CommonCurse,
            UncommonCurse,
            RareCurse
        }

        private string RarityName(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Rare:
                    return "Rare";
                case Rarity.Uncommon:
                    return "Uncommon";
                case Rarity.Common:
                    return "Common";
                case Rarity.RareCurse:
                    return "Rare Curse";
                case Rarity.UncommonCurse:
                    return "Uncommon Curse";
                case Rarity.CommonCurse:
                    return "Common Curse";
                default:
                    return "No Rarity?";
            }
        }
        private Rarity CardRarity(CardInfo cardInfo)
        {
            Rarity rarity;

            if (cardInfo.categories.Contains(CurseManager.instance.curseCategory))
            {
                switch (cardInfo.rarity)
                {
                    case CardInfo.Rarity.Rare:
                        rarity = Rarity.RareCurse;
                        break;
                    case CardInfo.Rarity.Uncommon:
                        rarity = Rarity.UncommonCurse;
                        break;
                    default:
                        rarity = Rarity.CommonCurse;
                        break;
                }
            }
            else
            {
                switch (cardInfo.rarity)
                {
                    case CardInfo.Rarity.Rare:
                        rarity = Rarity.Rare;
                        break;
                    case CardInfo.Rarity.Uncommon:
                        rarity = Rarity.Uncommon;
                        break;
                    default:
                        rarity = Rarity.Common;
                        break;
                }
            }

            return rarity;
        }

        private CardInfo RandomCard(CardInfo[] cards)
        {
            if (!(cards.Count() > 0))
            {
                return null;
            }

            return cards[random.Next(cards.Count())];
        }

        private static class WaitFor
        {
            public static IEnumerator Frames(int frameCount)
            {
                if (frameCount <= 0)
                {
                    throw new ArgumentOutOfRangeException("frameCount", "Cannot wait for less that 1 frame");
                }

                while (frameCount > 0)
                {
                    frameCount--;
                    yield return null;
                }
            }
        }
    }
}
