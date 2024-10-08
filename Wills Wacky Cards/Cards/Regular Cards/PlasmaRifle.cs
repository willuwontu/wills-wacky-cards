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
using WillsWackyManagers.UnityTools;

namespace WWC.Cards
{
    class PlasmaRifle : CustomCard, IConditionalCard
    {
        private static CardInfo card;
        public CardInfo Card { get => card; set { if (!card) { card = value; } } }

        public bool Condition(Player player, CardInfo card)
        {
            // Make sure that card exists and is plasma rifle
            if (!card || card != PlasmaRifle.card)
            {
                return true;
            }

            // Make sure that palyer exists, and that our path to our gun exists.
            if (!player || !player.data || !player.data.weaponHandler || !player.data.weaponHandler.gun)
            {
                return true;
            }

            // We're not allowed to take a plasma weapon if we're already a charged weapon.
            if (player.data.weaponHandler.gun.useCharge)
            {
                return false;
            }

            return true;
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.reloadTimeAdd = 0.25f;
            gun.attackSpeed = 0.5f/0.3f;
            gun.projectileColor = Color.cyan;

            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType"), CustomCardCategories.instance.CardCategory("WWC Gun Type"), 
                //TRTCardCategories.TRT_Traitor, TRTCardCategories.IgnoreMaxCardsCategory, TRTCardCategories.CannotDiscard, TRTCardCategories.TRT_DoNotDropOnDeath 
            };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.useCharge = true;
            gun.chargeNumberOfProjectilesTo += 0;
            gun.chargeSpeedTo = 3f;
            gun.dontAllowAutoFire = true;
            gun.chargeDamageMultiplier *= 3.5f;

            if (!player.GetComponentInChildren<PlasmaWeapon_Mono>())
            {
                var child = new GameObject("Plasma");
                child.transform.SetParent(player.transform);
                var plasmaWeapon = child.AddComponent<PlasmaWeapon_Mono>();
                GunChargePatch.Extensions.GunExtensions.GetAdditionalData(gun).chargeTime = 0.8f;
                characterStats.objectsAddedToPlayer.Add(child);
            }

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
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
                    positive = false,
                    stat = "Reload Time",
                    amount = "+0.25s",
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
