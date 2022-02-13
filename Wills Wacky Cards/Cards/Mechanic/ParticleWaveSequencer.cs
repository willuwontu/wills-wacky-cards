using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    class ParticleWaveSequencer : CustomCard
    {
        public static CardCategory upgradeBlock = CustomCardCategories.instance.CardCategory("Mechanic-Upgrade Block");
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { Mechanic.MechanicClass, upgradeBlock };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var upgrader = player.GetComponentInChildren<MechanicUpgrader>();
            
            if (upgrader)
            {
                upgrader.blockModifier.additionalBlocks_add += 1;
                upgrader.blockModifier.cdMultiplier_mult /= 1.1f;
                upgrader.upgradeTime += 2.5f;
            }
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Drives me crazy
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Particle Wave Sequencer";
        }
        protected override string GetDescription()
        {
            return "By adding a feedback loop to the quantum wave circuits, we can get another flux wave emitted before diverting the remaining power back into the capacitors.";
        }
        protected override GameObject GetCardArt()
        {
            GameObject art = null;

            //try
            //{
            //    art = WillsWackyCards.instance.WWCCards.LoadAsset<GameObject>("C_PortableFabricator");
            //    var cards = art.transform.Find("Foreground/Cards");

            //    foreach (Transform child in cards)
            //    {
            //        child.Find("Card Holder").gameObject.AddComponent<GetRandomCardVisualsOnEnable>();
            //    }
            //}
            //catch
            //{
            //    art = null;
            //}

            return art;
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
                    positive = true,
                    stat = "Block per Upgrade",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Block CD per Upgrade",
                    amount = "-10%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Upgrade Time",
                    amount = "+2.5s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.TechWhite;
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
