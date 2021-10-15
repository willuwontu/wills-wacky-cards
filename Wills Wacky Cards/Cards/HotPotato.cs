using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WillsWackyCards.Extensions;
using WillsWackyManagers.Utils;
using ModdingUtils.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WillsWackyCards.Cards
{
    class HotPotato : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseInteractionCategory };
            UnityEngine.Debug.Log($"[WWC][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(20, () =>
            {
                var curses = player.data.currentCards.Intersect(CurseManager.instance.GetRaw()).ToList();
                var curse = curses[UnityEngine.Random.Range(0, curses.Count - 1)];
                for (int i = 0; i < player.data.currentCards.Count; i++)
                {
                    if (player.data.currentCards[i].cardName == curse.cardName)
                    {
                        ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, i);
                        var randomEnemy = PlayerManager.instance.players.Where((person) => person.teamID != player.teamID).ToArray()[UnityEngine.Random.Range(0, PlayerManager.instance.players.Count - 2)];
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(randomEnemy, curse, false, "", 2f, 2f, true);
                        ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(randomEnemy, curse);
                        break;
                    }
                }
            });

            UnityEngine.Debug.Log($"[WWC][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[WWC][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Hot Potato";
        }
        protected override string GetDescription()
        {
            return "Pass along a curse to a \"friend\".";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Curse",
                    amount = "-1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }
        public override string GetModName()
        {
            return "WWC";
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
