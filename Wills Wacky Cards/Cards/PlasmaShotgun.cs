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
using UnityEngine.UI;

namespace WWC.Cards
{
    class PlasmaShotgun : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.reloadTimeAdd = 0.5f;
            gun.attackSpeed = 0.8f/0.3f;
            gun.numberOfProjectiles = 1;
            gun.projectileColor = Color.cyan;
            gun.destroyBulletAfter = 0.15f;
            gun.spread = 0.2f;
            gun.ammo = 5;

            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType"), CustomCardCategories.instance.CardCategory("WWC Gun Type") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.useCharge = true;
            gun.chargeNumberOfProjectilesTo += 10;
            gun.chargeSpreadTo += 0.5f;
            gun.chargeSpeedTo = 6f;
            gun.dontAllowAutoFire = true;
            gun.chargeDamageMultiplier *= 2f;

            if (!player.GetComponentInChildren<PlasmaWeapon_Mono>())
            {
                var child = new GameObject("Plasma");
                child.transform.SetParent(player.transform);
                var plasmaWeapon = child.AddComponent<PlasmaWeapon_Mono>();
                GunChargePatch.Extensions.GunExtensions.GetAdditionalData(gun).chargeTime = 1f;
                characterStats.objectsAddedToPlayer.Add(child);
            }

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.chargeSpreadTo -= 0.5f;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
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
                    amount = string.Format("-{0:F0}%", 0.8f/0.3f),
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
            return WillsWackyCards.ModInitials;
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
