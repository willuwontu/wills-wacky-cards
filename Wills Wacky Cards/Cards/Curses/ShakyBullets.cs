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
    class ShakyBullets : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseCategory };
            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.GetAdditionalData().amplitude += 0.25f;
            gun.GetAdditionalData().frequency += 0.125f;
            player.gameObject.GetOrAddComponent<WaveMover_Mono>();
            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<WaveMover_Mono>();
            UnityEngine.GameObject.Destroy(mono);
            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Shaky Bullets";
        }
        protected override string GetDescription()
        {
            return "Sometimes bullets have a little too much vodka.";
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
                    stat = "Bullets",
                    amount = "Shaky",
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
