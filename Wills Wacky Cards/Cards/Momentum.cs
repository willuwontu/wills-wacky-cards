using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WillsWackyCards.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WillsWackyCards.Cards
{
    class Momentum : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            MomentumTracker.stacks += 1;
            UnityEngine.Debug.Log($"[WWC][Card] {MomentumTracker.stacks} Momentum Stacks built up");
            gun.ammo = MomentumTracker.stacks;
            gun.attackSpeed = (float) Math.Pow(1.05f, (double) MomentumTracker.stacks);
            gun.projectileSpeed = (float) Math.Pow(1.05f, (double)MomentumTracker.stacks);
            gun.reflects = MomentumTracker.stacks;
            gun.damage = (float) Math.Pow(1.05f, (double) MomentumTracker.stacks);
            gun.bursts = MomentumTracker.stacks / 10;
            gun.numberOfProjectiles = MomentumTracker.stacks / 5;

            statModifiers.movementSpeed = (float) Math.Pow(1.05f, (double)MomentumTracker.stacks); ;
            statModifiers.lifeSteal = (float) Math.Pow(1.05f, (double)MomentumTracker.stacks); ;

            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("CardManipulation") };

            cardInfo.cardStats = MomentumTracker.GetMomentumStats();
            UnityEngine.Debug.Log("[WWC][Card] Momentum Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            if (MomentumTracker.playerStacks.TryGetValue(player, out var stacks))
            {
                stacks += MomentumTracker.stacks;
            }
            else
            {
                MomentumTracker.playerStacks.Add(player, MomentumTracker.stacks);
            }

            //MomentumTracker.stacks = 0;
        }
        public override void OnRemoveCard()
        {
            //Drives me crazy
        }

        protected override string GetTitle()
        {
            return "Momentum";
        }
        protected override string GetDescription()
        {
            return "Stats increase each time it appears without being taken.";
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
            return MomentumTracker.GetMomentumStats();
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
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
    public class MomentumTracker
    {
        public static int stacks = 0;
        public static Dictionary<Player, int> playerStacks = new Dictionary<Player, int>();
        public static Dictionary<Player, bool> trackRemoval = new Dictionary<Player, bool>();

        public static CardInfoStat[] GetMomentumStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Ammo",
                    amount = $"+{MomentumTracker.stacks}",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Attack Speed",
                    amount = string.Format("+{0:F0}%",((float) Math.Pow(1.05f, (double) MomentumTracker.stacks) -1f)* 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet Speed",
                    amount = string.Format("+{0:F0}%",((float) Math.Pow(1.05f, (double) MomentumTracker.stacks) -1f)* 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bounces",
                    amount = $"+{MomentumTracker.stacks}",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bursts",
                    amount = $"+{MomentumTracker.stacks/10}",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullets",
                    amount = $"+{MomentumTracker.stacks/5}",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Move Speed",
                    amount = string.Format("+{0:F0}%",((float) Math.Pow(1.05f, (double) MomentumTracker.stacks) -1f)* 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Life Steal",
                    amount = string.Format("+{0:F0}%",((float) Math.Pow(1.05f, (double) MomentumTracker.stacks) -1f)* 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
    }
}
