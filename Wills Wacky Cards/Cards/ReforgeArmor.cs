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
    class ReforgeArmor : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.NeedsNull();
            cardInfo.MarkUnNullable();
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            int nullcount = player.GetNullCount();
            int epicNullcount = 0;
            RarityLib.Utils.RarityUtils.Rarities.Values.ToList().ForEach(r => {
                if (r.relativeRarity <= RarityLib.Utils.RarityUtils.GetRarityData(Rarities.Epic).relativeRarity)
                {
                    epicNullcount += player.GetNullCount(r.value);
                }
            });

            var nullData = characterStats.GetAdditionalData().nullData;

            nullData.damageRedCards += 1;

            characterStats.GetAdditionalData().DamageReduction += ((0.5f * Mathf.Log10(nullData.damageRedCards * nullcount + 1)) - (0.5f * Mathf.Log10((nullData.damageRedCards - 1) * nullcount + 1)));
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
            return "Nulled cards you have now provide damage reduction.";
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