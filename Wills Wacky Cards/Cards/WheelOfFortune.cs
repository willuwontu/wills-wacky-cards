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
using TMPro;

namespace WWC.Cards
{
    class WheelOfFortune : CustomCard
    {
        public static CardInfo card;
        public override void Callback()
        {
            var mono = gameObject.AddComponent<WheelOfFortune_Mono>();
            mono.currentRarity = RarityLib.Utils.RarityUtils.Rarities.Values.ToArray()[UnityEngine.Random.Range(0, RarityLib.Utils.RarityUtils.Rarities.Values.ToArray().Length)].value;
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("CardManipulation") };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(5, () =>
            {
                WheelOfFortune_Mono.Picked(player);
            });

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Wheel of Fortune";
        }
        protected override string GetDescription()
        {
            return "Lorem Ipsum I can't remember the rest of this phrase for testing paragraph lengths and looks.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return WillsWackyCards.EpicRarity;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {

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

namespace WWC.MonoBehaviours
{
    public class WheelOfFortune_Mono : MonoBehaviour
    {
        private TextMeshProUGUI description;
        private TextMeshProUGUI cardName;
        public string title;
        public CardInfo.Rarity currentRarity;
        public bool locked = false;
        private float minDuration = 0.05f;
        private float maxDuration = 0.2f;
        private float nextCheck = 0f;

        private StatHolder[] stats = new StatHolder[] { };

        private class StatHolder
        {
            public TextMeshProUGUI stat;
            public TextMeshProUGUI value;
        }

        private void Awake()
        {

        }
        private void Start()
        {
            TextMeshProUGUI[] allChildrenRecursive = this.gameObject.GetComponentsInChildren<TextMeshProUGUI>();

            if (allChildrenRecursive.Length < 1)
            {
                return;
            }

            GameObject effectText = allChildrenRecursive.Where(obj => obj.gameObject.name == "EffectText").FirstOrDefault().gameObject;
            GameObject titleText = allChildrenRecursive.Where(obj => obj.gameObject.name == "Text_Name").FirstOrDefault().gameObject;

            this.description = effectText.GetComponent<TextMeshProUGUI>();
            this.cardName = titleText.GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
            if (!this.cardName)
            {
                return;
            }

            if (this.nextCheck > Time.time && !this.locked)
            {
                return;
            }

            Player player = null;

            if (CardChoice.instance.IsPicking)
            {
                player = PlayerManager.instance.GetPlayerWithID(CardChoice.instance.pickrID);
            }

            if (!this.locked)
            {
                CardInfo.Rarity[] availableRarities = UnboundLib.Utils.CardManager.cards.Values.Where(card => card.enabled).Select(card => card.cardInfo).Where(cardInfo => ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, cardInfo)).Select(cardInfo => cardInfo.rarity).Distinct().ToArray();

                if (!(availableRarities.Length > 0))
                {
                    return;
                }

                int rarityWeight = availableRarities.Select(rarity => CardAmountByRarity(rarity)).Sum();
                int randomWeight = UnityEngine.Random.Range(0, rarityWeight + 1);

                availableRarities.Shuffle();

                for (int i = 0; i < availableRarities.Length; i++)
                {
                    randomWeight -= CardAmountByRarity(availableRarities[i]);

                    if (randomWeight <= 0)
                    {
                        this.currentRarity = availableRarities[i];
                        break;
                    }
                }
            }

            var amount = CardAmountByRarity(this.currentRarity);

            Color origColor = RarityLib.Utils.RarityUtils.GetRarityData(CardInfo.Rarity.Common).color;

            RarityLib.Utils.RarityUtils.GetRarityData(CardInfo.Rarity.Common).color = new Color(0.8f, 0.8f, 0.8f, 1f);

            this.description.text = $"You win {amount} <color=#{ColorUtility.ToHtmlStringRGB(RarityLib.Utils.RarityUtils.GetRarityData(this.currentRarity).color)}>{this.currentRarity.ToString()}</color> card{(amount > 1 ? "s" : "")}!";

            RarityLib.Utils.RarityUtils.GetRarityData(CardInfo.Rarity.Common).color = origColor;

            this.nextCheck = Time.time + UnityEngine.Random.Range(this.minDuration, this.maxDuration);

            if (!CardChoice.instance.IsPicking)
            {
                nextCheck = Time.time + (UnityEngine.Random.Range(this.minDuration, this.maxDuration) * 10f);
            }
        }
        public static void Picked(Player player)
        {
            CardInfo[] availableCards = UnboundLib.Utils.CardManager.cards.Values.Where(card => card.enabled).Select(card => card.cardInfo).Where(cardInfo => ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, cardInfo) && (cardInfo != WWC.Cards.WheelOfFortune.card) && (!cardInfo.categories.Contains(WillsWackyManagers.Utils.CurseManager.instance.curseCategory))).ToArray();
            CardInfo.Rarity[] availableRarities = availableCards.Select(cardInfo => cardInfo.rarity).Distinct().ToArray();

            int rarityWeight = availableRarities.Select(rarity => CardAmountByRarity(rarity)).Sum();
            int randomWeight = UnityEngine.Random.Range(0, rarityWeight + 1);

            WillsWackyCards.SendDebugLog($"Player {player.playerID} has a total of {availableCards.Length} cards and {availableRarities.Length} distinct rarities available to them.",true);
            WillsWackyCards.SendDebugLog($"The rarities have the following rewards:\n{availableRarities.Select(rarity => $"{rarity.ToString()}: {WheelOfFortune_Mono.CardAmountByRarity(rarity)} card{(WheelOfFortune_Mono.CardAmountByRarity(rarity) != 1 ? "s" : "")};  Odds: {WheelOfFortune_Mono.CardAmountByRarity(rarity)}/{rarityWeight}")}",true);

            availableRarities.Shuffle();
            availableRarities.Shuffle();

            CardInfo.Rarity chosenRarity = CardInfo.Rarity.Common;

            for (int i = 0; i < availableRarities.Length; i++)
            {
                randomWeight -= CardAmountByRarity(availableRarities[i]);

                if (randomWeight <= 0)
                {
                    chosenRarity = availableRarities[i];
                    break;
                }
            }
            WillsWackyCards.SendDebugLog($"Player {player.playerID} rolled {chosenRarity.ToString()} rarity for their wheel.", true);
            CardInfo[] rarityCards = availableCards.Where(card => card.rarity == chosenRarity).ToArray();
            WillsWackyCards.SendDebugLog($"There {(rarityCards.Length != 1 ? "are" : "is")}  {rarityCards.Length} card{(rarityCards.Length != 1 ? "s" : "")} available in that rarity for the player.", true);

            List<CardInfo> newCards = new List<CardInfo>();
            int amount = WheelOfFortune_Mono.CardAmountByRarity(chosenRarity);
            WillsWackyCards.SendDebugLog($"Rolling {amount} {chosenRarity.ToString()} card{(amount > 1 ? "s" : "")} for Player {player.playerID} now.", true);

            for (int i = 0; i < amount; i++)
            {
                CardInfo newCard = null;

                CardInfo[] allValidCards = rarityCards.Where(c => (c != WWC.Cards.WheelOfFortune.card) && (ModdingUtils.Utils.Cards.instance.CardDoesNotConflictWithCards(c, newCards.ToArray())) && (ModdingUtils.Utils.Cards.instance.PlayerIsAllowedCard(player, c))).ToArray();

                WillsWackyCards.SendDebugLog($"There are {(allValidCards.Length != 1 ? "are" : "is")} valid {allValidCards.Length} card{(allValidCards.Length != 1 ? "s" : "")} available for the player to randomly roll into.", true);

                if (allValidCards.Length > 0)
                {
                    newCard = allValidCards[UnityEngine.Random.Range(0, allValidCards.Length)];
                    newCards.Add(newCard);
                    WillsWackyCards.SendDebugLog($"{newCard.cardName} ({newCard.gameObject.name}) was chosen.", true);
                }
            }

            ModdingUtils.Utils.Cards.instance.AddCardsToPlayer(player, newCards.ToArray(), false, null, null, null);
        }
        public static int CardAmountByRarity(CardInfo.Rarity rarity)
        {
            int result = 0;

            RarityLib.Utils.Rarity[] rarities = RarityLib.Utils.RarityUtils.Rarities.Values.OrderBy(r => r.relativeRarity).ThenBy(r => r.name).ToArray().ToArray();

            float value = rarities.Where(r => r.value == rarity).First().relativeRarity;
            float min = rarities.Select(r => r.relativeRarity).Min();
            float max = 0.8f;
            float sum = rarities.Select(r => r.relativeRarity).Sum();

            result = ((int)(Mathf.Round((value/max) * 3f))) + 1;

            return result;
        }
    }
}