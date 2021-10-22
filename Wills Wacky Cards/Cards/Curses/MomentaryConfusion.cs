using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WillsWackyCards.Extensions;
using WillsWackyCards.MonoBehaviours;
using WillsWackyManagers.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WillsWackyCards.Cards.Curses
{
    class MomentaryConfusion : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            UnityEngine.Debug.Log($"[WWC][Curse] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var confuzzle = player.gameObject.GetOrAddComponent<MomentaryConfusion_Mono>();
            confuzzle.chance += 10;
            confuzzle.bufferTime = Mathf.Max(2, confuzzle.bufferTime - 2f);
            confuzzle.duration += 2f;
            confuzzle.timeBetweenChecks = Mathf.Max(0.2f, confuzzle.timeBetweenChecks - 0.2f);
            UnityEngine.Debug.Log($"[WWC][Curse] {GetTitle()} added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var confuzzle = player.gameObject.GetComponent<MomentaryConfusion_Mono>();
            if (confuzzle)
            {
                confuzzle.chance -= 10;
                confuzzle.bufferTime = Mathf.Max(2, confuzzle.bufferTime + 2f);
                confuzzle.duration -= 2f;
                confuzzle.timeBetweenChecks = Mathf.Max(0.2f, confuzzle.timeBetweenChecks + 0.2f); 
            }
            UnityEngine.Debug.Log($"[WWC][Curse] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Momentary Confusion";
        }
        protected override string GetDescription()
        {
            return "Sometimes you just do things differently.";
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
                    positive = false,
                    stat = "Confusion",
                    amount = "Occasional",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }
        public override string GetModName()
        {
            return "Curse";
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
