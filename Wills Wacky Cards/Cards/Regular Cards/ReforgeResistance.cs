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
using Nullmanager;
using UnityEngine;

namespace WWC.Cards
{
    class ReforgeResistance : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.NeedsNull();
            cardInfo.MarkUnNullable();
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            int nullcount = player.GetNullCount();
            int rareNullcount = 0;
            RarityLib.Utils.RarityUtils.Rarities.Values.ToList().ForEach(r => {
                if (r.relativeRarity <= RarityLib.Utils.RarityUtils.GetRarityData(CardInfo.Rarity.Rare).relativeRarity)
                {
                    rareNullcount += player.GetNullCount(r.value);
                }
            });

            characterStats.GetAdditionalData().nullData.willPowerMult *= 1.1f;
            characterStats.GetAdditionalData().nullData.poisonResMult *= 0.95f;
            characterStats.GetAdditionalData().willpower *= Mathf.Pow(1.1f, nullcount);
            characterStats.GetAdditionalData().poisonResistance *= Mathf.Pow(0.95f, nullcount);
            WillsWackyCards.UpdateNullStatsForPlayer(player);
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            
        }

        protected override string GetTitle()
        {
            return "Reforge Resistance";
        }
        protected override string GetDescription()
        {
            return "Nulled cards you have now provide +5% poison resistance and +10% willpower.";
        }
        protected override GameObject GetCardArt()
        {
            GameObject art;

            try
            {
                art = WillsWackyManagers.WillsWackyManagers.instance.WWMAssets.LoadAsset<GameObject>("C_Reforgeresistance");
            }
            catch
            {
                art = null;
            }

            return art;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return Rarities.Trinket;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {

            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
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