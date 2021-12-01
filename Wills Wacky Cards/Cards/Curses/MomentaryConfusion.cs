using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using WillsWackyManagers.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards.Curses
{
    class MomentaryConfusion : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseCategory };
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var confuzzle = player.gameObject.GetOrAddComponent<MomentaryConfusion_Mono>();
            confuzzle.chance += 10;
            confuzzle.bufferTime = Mathf.Max(2, confuzzle.bufferTime - 2f);
            confuzzle.duration += 2f;
            confuzzle.timeBetweenChecks = Mathf.Max(0.2f, confuzzle.timeBetweenChecks - 0.2f);
            confuzzle.cardName = GetTitle();
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} added to Player {player.playerID}");
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
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} removed from Player {player.playerID}");
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
            return CardInfo.Rarity.Uncommon;
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
            return WillsWackyCards.CurseInitials;
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
