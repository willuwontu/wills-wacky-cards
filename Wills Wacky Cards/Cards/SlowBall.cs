﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using WillsWackyCards.Extensions;

namespace WillsWackyCards.Cards
{
    class SlowBall : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.GetAdditionalData().speedDamageMultiplier -= 0.15f;
            gun.reloadTimeAdd = 0.15f;
            UnityEngine.Debug.Log("[WWC][Card] Alternate Universe Physics Built");
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
            return "Alternate Universe Physics";
        }
        protected override string GetDescription()
        {
            return "F = M/A";
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
                    amount = "Slower Bullets",
                    stat = "Increase Damage",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload Time",
                    amount = "+0.15s",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
        }
        public override string GetModName()
        {
            return "WWC";
        }
    }
}