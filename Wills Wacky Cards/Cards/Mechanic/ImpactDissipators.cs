﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using WWC.Interfaces;
using UnityEngine;
using ClassesManagerReborn.Util;

namespace WWC.Cards
{
    class ImpactDissipators : CustomMechanicCard
    {
        protected override GameObject GetAccessory()
        {
            return WillsWackyManagers.WillsWackyManagers.instance.WWMAssets.LoadAsset<GameObject>("Shield Accessory");
        }
        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = MechanicClass.name;
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var upgrader = player.GetComponentInChildren<MechanicUpgrader>();

            if (upgrader)
            {
                upgrader.characterStatModifiersModifier.secondsToTakeDamageOver_add += 4f;
                upgrader.upgradeCooldownMult *= 1.25f;
            }

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        internal static CardInfo card = null;
        protected override string GetTitle()
        {
            return "Impact Dissipators";
        }
        protected override string GetDescription()
        {
            return $"By inventing time travel, we can distribute unwanted forces over it.";
        }
        protected override GameObject GetCardArt()
        {
            GameObject art;

            try
            {
                art = WillsWackyManagers.WillsWackyManagers.instance.WWMAssets.LoadAsset<GameObject>("C_ImpactDissipators");
            }
            catch
            {
                art = null;
            }

            return art;
        }
        protected override GameObject GetCardBase()
        {
            return Mechanic.cardBase;
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
                    stat = "Damage taken over per upgrade",
                    amount = "+4s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Upgrade Cooldown",
                    amount = "+25%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.NatureBrown;
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