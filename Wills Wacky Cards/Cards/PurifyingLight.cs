using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using ModdingUtils.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using WillsWackyManagers.Utils;
using UnityEngine;

namespace WWC.Cards
{
    class PurifyingLight : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseInteractionCategory, RerollManager.instance.NoFlip };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => {
                WillsWackyCards.instance.StartCoroutine(IReplaceCurses(player, gun, gunAmmo, data, health, gravity, block, characterStats)); 
            });

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        private IEnumerator IReplaceCurses(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var curseIndeces = Enumerable.Range(0, player.data.currentCards.Count()).Where((index) => CurseManager.instance.IsCurse(player.data.currentCards[index])).ToList();

            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(CurseManager.instance.curseInteractionCategory);

            var replacements = new List<CardInfo>();

            foreach (int index in curseIndeces)
            {
                Func<CardInfo, Player, Gun, GunAmmo, CharacterData, HealthHandler, Gravity, Block, CharacterStatModifiers, bool> condition = (card, p, g, ga, d, h, gr, b, s) =>
                {
                    var result = true;

                    if (card.rarity != CardInfo.Rarity.Common)
                    {
                        result = false;
                    }

                    if (card.categories.Contains(CurseManager.instance.curseInteractionCategory))
                    {
                        result = false;
                    }

                    if (!ModdingUtils.Utils.Cards.instance.CardDoesNotConflictWithCards(card, replacements.ToArray()))
                    {
                        result = false;
                    }

                    if (!ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(card).canBeReassigned)
                    {
                        result = false;
                    }

                    return result;
                };

                var replacement = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(player, gun, gunAmmo, data, health, gravity, block, characterStats, condition);

                replacements.Add(replacement);
            }

            yield return ModdingUtils.Utils.Cards.instance.ReplaceCards(player, curseIndeces.ToArray(), replacements.ToArray(), null, true);

            // yield return ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, replacements.ToArray(), replacements.ToArray().Length * 1f);

            yield break;
        }

        private void ReplaceCurse(CardInfo curse, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            Func<CardInfo, Player, Gun, GunAmmo, CharacterData, HealthHandler, Gravity, Block, CharacterStatModifiers, bool> condition;

            switch (curse.rarity)
            {
                case CardInfo.Rarity.Rare:
                    condition = RareCondition;
                    break;
                case CardInfo.Rarity.Uncommon:
                    condition = UncommonCondition;
                    break;
                default:
                    condition = CommonCondition;
                    break;
            }

            condition = CommonCondition;

            var replacement = ModdingUtils.Utils.Cards.instance.GetRandomCardWithCondition(player, gun, gunAmmo, data, health, gravity, block, characterStats, condition);

            if (replacement == null)
            {
                UnityEngine.Debug.Log($"[WWC][Debugging] Purifying Light replacement is null.");
            }

            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, replacement, false, "", 2f, 2f, true);
        }

        private bool CommonCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var result = true;

            if (card.rarity != CardInfo.Rarity.Common)
            {
                result = false;
            }

            if (card.categories.Contains(CurseManager.instance.curseInteractionCategory))
            {
                result = false;
            }



            return result;
        }
        private bool UncommonCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            return card.rarity == CardInfo.Rarity.Uncommon;
        }
        private bool RareCondition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {

            return card.rarity == CardInfo.Rarity.Rare;
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Drives me crazy
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Purifying Light";
        }
        protected override string GetDescription()
        {
            return "Replace each curse with a random common.";
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
                    stat = "Curses Removed",
                    amount = "All",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.PoisonGreen;
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
