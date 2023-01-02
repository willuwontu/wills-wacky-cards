using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using WWC.Extensions;

namespace WWC.Cards
{
    class BulletBridge : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.ammo = 10;
            gun.bursts = 10;
            gun.timeBetweenBullets = 0.3f;
            gun.attackSpeed = 0.1f;
            //gun.GetAdditionalData().useAttacksPerAttack = true;
            //gun.GetAdditionalData().attacksPerAttack = 3;
            //gun.spread = .5f;
            //gun.evenSpread = .5f;
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
            return "Bullet Bridge";
        }
        protected override string GetDescription()
        {
            return "Make a bridge of bullets";
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
                    positive = true,
                    stat = "Ammo",
                    amount = "+10",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullets",
                    amount = "+10",
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
            return WillsWackyCards.ModInitials;
        }
    }
}
