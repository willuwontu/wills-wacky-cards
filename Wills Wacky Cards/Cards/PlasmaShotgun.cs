﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WillsWackyCards.Extensions;
using WillsWackyCards.MonoBehaviours;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WillsWackyCards.Cards
{
    class PlasmaShotgun : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.reloadTimeAdd = 1.5f;
            gun.attackSpeed = 0.8f/0.3f;
            gun.numberOfProjectiles = 1;
            gun.projectileColor = Color.cyan;
            gun.destroyBulletAfter = 0.15f;
            gun.spread = 0.2f;
            gun.ammo = 5;

            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType"), CustomCardCategories.instance.CardCategory("WWC Gun Type") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
            UnityEngine.Debug.Log("[WWC][Card] Plasma Shotgun Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.useCharge = true;
            gun.chargeNumberOfProjectilesTo += 10;
            gun.chargeSpreadTo += 0.5f;
            gun.chargeSpeedTo = 5f;
            gun.dontAllowAutoFire = true;
            gun.chargeDamageMultiplier *= 1f;
            gun.GetAdditionalData().chargeTime = 1f;

            var chargeBar = Instantiate(player.transform.Find("WobbleObjects/Healthbar"), player.transform.Find("WobbleObjects"));
            chargeBar.name = "ChargeBar";
            chargeBar.Translate(new Vector3(.95f, -1.1f, 0));
            chargeBar.localScale.Set(0.5f, 1f, 1f);
            chargeBar.localScale = new Vector3(0.6f, 1.4f, 1f);
            chargeBar.Rotate(0f, 0f, 90f);
            var plasmaShotgun = player.gameObject.AddComponent<PlasmaWeapon_Mono>();
        }
        public override void OnRemoveCard()
        {
            //Drives me crazy
        }

        protected override string GetTitle()
        {
            return "Plasma Shotgun";
        }
        protected override string GetDescription()
        {
            return "Good for exterminating aliens.";
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
                    stat = "Charged Attacks",
                    amount = "Use",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Charge Bullets",
                    amount = "+10",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Ammo",
                    amount = "+5",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Attack Speed",
                    amount = string.Format("+{0:F0}%", 0.8f/0.3f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload Time",
                    amount = "+1.5s",
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
            return "WWC";
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}