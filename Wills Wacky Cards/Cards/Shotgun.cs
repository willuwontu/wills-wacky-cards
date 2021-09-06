using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using WillsWackyCards.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

namespace WillsWackyCards.Cards
{
    class Shotgun : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.ammo = 9;
            gun.gravity = 0.5f;
            gun.numberOfProjectiles = 4;
            gun.projectileSpeed = 1.5f;
            gun.damage = 0.8f;
            gun.spread = .2f;
            gun.evenSpread = .5f;
            gun.recoilMuiltiplier = 5f;
            gun.destroyBulletAfter = 1.5f;
            gun.timeBetweenBullets = 0.25f;
            gun.GetAdditionalData().useForcedReloadSpeed = true;
            gun.GetAdditionalData().forcedReloadSpeed = 5.0f;
            gun.GetAdditionalData().attacksPerAttack = 2;
            gun.GetAdditionalData().useAttacksPerAttack = true;
            gun.GetAdditionalData().useForcedAttackSpeed = true;
            gun.forceSpecificAttackSpeed = 1f;

            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
            UnityEngine.Debug.Log("[WWC][Card] Shotgun Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //throw new NotImplementedException();
        }
        public override void OnRemoveCard()
        {
            //throw new NotImplementedException();
        }

        protected override string GetTitle()
        {
            return "Shotgun";
        }
        protected override string GetDescription()
        {
            return "It's Double-Barreled!";
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
                    positive = true,
                    stat = "Attack",
                    amount = "Double",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullets",
                    amount = "+4",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Ammo",
                    amount = "+9",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Forced Reload Time",
                    amount = "5s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Forced Attack Speed",
                    amount = "1s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Damage",
                    amount = "-20%",
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
    }
}
