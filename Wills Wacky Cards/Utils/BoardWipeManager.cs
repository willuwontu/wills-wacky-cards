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
    public class BoardWipeManager : MonoBehaviour
    {
        public static BoardWipeManager instance { get; private set; }
        public CardCategory NoFlip = CustomCardCategories.instance.CardCategory("NoFlip");
        //The player initiating the board flip
        public Player flippingPlayer;
        public bool tableFlipped;
        public CardInfo tableFlipCard;
        public Player rerollPlayer;
        public bool reroll;
        public CardInfo rerollCard;
        private System.Random random = new System.Random();


        public void Start()
        {
            instance = this;
        }

        public IEnumerator FlipTable(bool addCard = true)
        {
            Dictionary<Player, List<Rarity>> cardRarities = new Dictionary<Player, List<Rarity>>();

            foreach (var player in PlayerManager.instance.players)
            {
                UnityEngine.Debug.Log($"Getting card rarities for player {player.playerID}");
                // Compile List of Rarities
                cardRarities.Add(player, player.data.currentCards.Select(card => CardRarity(card)).ToList());
                ModdingUtils.Utils.Cards.instance.RemoveAllCardsFromPlayer(player);
                UnityEngine.Debug.Log($"{cardRarities[player].Count} card rarities found for player {player.playerID}");
            }

            if (flippingPlayer)
            {
                // Remove the last card from the flipping player, since it's going to be board wipe
                cardRarities[flippingPlayer].RemoveAt(cardRarities[flippingPlayer].Count - 1);
            }

            var allCards = CardManager.cards.Values.ToArray().Where(cardData => cardData.enabled && !(cardData.cardInfo.categories.Contains(NoFlip) || (cardData.cardInfo.cardName.ToLower() == "shuffle"))).Select(card => card.cardInfo).ToList();
            allCards.Remove(tableFlipCard);

            UnityEngine.Debug.Log($"{allCards.Count()} cards are enabled and ready to be swapped out.");

            yield return WillsWackyCards.WaitFor.Frames(1);

            for (int i = 0; i < Mathf.Max(cardRarities.Values.Select(cards => cards.Count()).ToArray()); i++)
            {
                UnityEngine.Debug.Log($"Initiating round {i+1} of readding cards to players.");
                foreach (var player in PlayerManager.instance.players)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.RemoveAll(category => category == CurseManager.instance.curseCategory);
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseManager.instance.curseInteractionCategory);
                    if (CurseManager.instance.HasCurse(player))
                    {
                        ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.RemoveAll(category => category == CurseManager.instance.curseInteractionCategory);
                        UnityEngine.Debug.Log($"Player {player.playerID} is available for curse interaction effects");
                    }
                    else if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Contains(CurseManager.instance.curseInteractionCategory))
                    {
                        ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseManager.instance.curseInteractionCategory);
                    }
                    UnityEngine.Debug.Log($"Checking player {player.playerID} to see if they are able to have a card added.");
                    if (i < cardRarities[player].Count)
                    {
                        UnityEngine.Debug.Log($"Player {player.playerID} is able to have a card added.");
                        var rarity = cardRarities[player][i];
                        UnityEngine.Debug.Log($"Player {player.playerID}'s card was originally {RarityName(rarity)}, finding a replacement now.");
                        var cardChoices = allCards.Where(cardInfo => (CardRarity(cardInfo) == rarity) && (ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, cardInfo))).ToArray();
                        UnityEngine.Debug.Log($"Player {player.playerID} is eligible for {cardChoices.Count()} cards");
                        if (cardChoices.Count() > 0)
                        {
                            var card = RandomCard(cardChoices);
                            UnityEngine.Debug.Log($"Player {player.playerID} is being given {card.cardName}");
                            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, false, "", 2f, 2f, true);
                            ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, card);
                        }
                    }
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseManager.instance.curseCategory);
                    yield return WillsWackyCards.WaitFor.Frames(10);
                }

                yield return WillsWackyCards.WaitFor.Frames(20); 
            }
            UnityEngine.Debug.Log("Finished adding cards to players.");

            if (flippingPlayer && tableFlipCard && addCard)
            {
                // Add the tableflip card to the player
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(flippingPlayer, tableFlipCard, false, "", 2f, 2f, true);
                ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(flippingPlayer, tableFlipCard);
            }
            yield return instance.StartCoroutine(WillsWackyCards.WaitFor.Frames(20));

            flippingPlayer = null;
            tableFlipped = false;
            yield return null;
        }

        public IEnumerator Reroll(bool addCard = true)
        {
            if (rerollPlayer)
            {
                List<Rarity> cardRarities = new List<Rarity>();

                UnityEngine.Debug.Log($"Getting card rarities for player {rerollPlayer.playerID}");
                cardRarities = rerollPlayer.data.currentCards.Select(card => CardRarity(card)).ToList();
                UnityEngine.Debug.Log($"{cardRarities.Count} card rarities found for player {rerollPlayer.playerID}");
                ModdingUtils.Utils.Cards.instance.RemoveAllCardsFromPlayer(rerollPlayer);

                // Remove the last card from the flipping player, since it's going to be rerolling
                cardRarities.RemoveAt(cardRarities.Count - 1);

                var allCards = CardManager.cards.Values.ToArray().Where(cardData => cardData.enabled && !(cardData.cardInfo.categories.Contains(NoFlip) || (cardData.cardInfo.cardName.ToLower() == "shuffle"))).Select(card => card.cardInfo).ToList();

                UnityEngine.Debug.Log($"{allCards.Count()} cards are enabled and ready to be swapped out.");

                yield return WillsWackyCards.WaitFor.Frames(1);


                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(rerollPlayer.data.stats).blacklistedCategories.RemoveAll(category => category == CurseManager.instance.curseCategory);

                foreach (var rarity in cardRarities)
                {

                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(rerollPlayer.data.stats).blacklistedCategories.Add(CurseManager.instance.curseInteractionCategory);
                    if (CurseManager.instance.HasCurse(rerollPlayer))
                    {
                        ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(rerollPlayer.data.stats).blacklistedCategories.RemoveAll(category => category == CurseManager.instance.curseInteractionCategory);
                        UnityEngine.Debug.Log($"Player {rerollPlayer.playerID} is available for curse interaction effects");
                    }
                    else if (!ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(rerollPlayer.data.stats).blacklistedCategories.Contains(CurseManager.instance.curseInteractionCategory))
                    {
                        ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(rerollPlayer.data.stats).blacklistedCategories.Add(CurseManager.instance.curseInteractionCategory);
                    }

                    UnityEngine.Debug.Log($"Checking player {rerollPlayer.playerID} to see if they are able to have a card added.");
                    UnityEngine.Debug.Log($"Player {rerollPlayer.playerID} is able to have a card added.");
                    UnityEngine.Debug.Log($"Player {rerollPlayer.playerID}'s card was originally {RarityName(rarity)}, finding a replacement now.");
                    var cardChoices = allCards.Where(cardInfo => (CardRarity(cardInfo) == rarity) && (ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(rerollPlayer, cardInfo))).ToArray();
                    UnityEngine.Debug.Log($"Player {rerollPlayer.playerID} is eligible for {cardChoices.Count()} cards");
                    if (cardChoices.Count() > 0)
                    {
                        var card = RandomCard(cardChoices);
                        UnityEngine.Debug.Log($"Player {rerollPlayer.playerID} is being given {card.cardName}");
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(rerollPlayer, card, false, "", 2f, 2f, true);
                        ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(rerollPlayer, card);
                    }

                    yield return WillsWackyCards.WaitFor.Frames(20);
                }
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(rerollPlayer.data.stats).blacklistedCategories.Add(CurseManager.instance.curseCategory);
                UnityEngine.Debug.Log("Finished adding cards to players.");

                if (rerollCard && addCard)
                {
                    // Add the tableflip card to the player
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(flippingPlayer, rerollCard, false, "", 2f, 2f, true);
                    ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(flippingPlayer, rerollCard);
                }
                yield return instance.StartCoroutine(WillsWackyCards.WaitFor.Frames(20)); 
            }

            rerollPlayer = null;
            reroll = false;
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
    }
}
