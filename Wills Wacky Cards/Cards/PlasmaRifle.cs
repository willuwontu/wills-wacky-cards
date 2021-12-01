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
    class PlasmaRifle : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.reloadTimeAdd = 0.25f;
            gun.attackSpeed = 0.5f/0.3f;
            gun.projectileColor = Color.cyan;

            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType"), CustomCardCategories.instance.CardCategory("WWC Gun Type") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.useCharge = true;
            gun.chargeNumberOfProjectilesTo += 0;
            gun.chargeSpeedTo = 2f;
            gun.dontAllowAutoFire = true;
            gun.chargeDamageMultiplier *= 3.5f;
            gun.GetAdditionalData().chargeTime = 0.8f;

            if (!player.GetComponent<PlasmaWeapon_Mono>())
            {
                var chargeBar = Instantiate(player.transform.Find("WobbleObjects/Healthbar"), player.transform.Find("WobbleObjects"));
                chargeBar.name = "ChargeBar";
                chargeBar.Translate(new Vector3(.95f, -1.1f, 0));
                chargeBar.localScale.Set(0.5f, 1f, 1f);
                chargeBar.localScale = new Vector3(0.6f, 1.4f, 1f);
                chargeBar.Rotate(0f, 0f, 90f);
                var plasmaRifle = player.gameObject.GetOrAddComponent<PlasmaWeapon_Mono>();
                var nameLabel = chargeBar.transform.Find("Canvas/PlayerName").gameObject;
                var crown = chargeBar.transform.Find("Canvas/CrownPos").gameObject;

                plasmaRifle.chargeImage = chargeBar.transform.Find("Canvas/Image/Health").GetComponent<Image>();
                plasmaRifle.whiteImage = chargeBar.transform.Find("Canvas/Image/White").GetComponent<Image>();
                plasmaRifle.whiteImage.SetAlpha(0);
                plasmaRifle.whiteImage.name = "Charge";
                plasmaRifle.chargeImage.color = new Color(255, 255, 255);
                plasmaRifle.chargeImage.SetAlpha(1);
                Destroy(nameLabel);
                Destroy(crown);
            }

            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.chargeDamageMultiplier /= 3.5f;
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Plasma Rifle";
        }
        protected override string GetDescription()
        {
            return "May as well be a space bounty hunter.";
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
                    stat = "Charged Shots",
                    amount = "Use",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Charge Damage",
                    amount = "+250%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Charge Bullet Speed",
                    amount = "+200%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Reload",
                    amount = "0.25%",
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
