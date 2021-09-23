using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using WillsWackyCards.Extensions;
using WillsWackyCards.MonoBehaviours;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

namespace WillsWackyCards.Cards
{
    class Vampirism : CustomCard
    {
        private Vampirism_Mono vampirism;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            statModifiers.gravity = 0.7f;
            gun.projectileColor = Color.red;
            statModifiers.lifeSteal = 1.0f;
            statModifiers.jump = 1.15f;
            statModifiers.movementSpeed = 1.15f;
            statModifiers.health = 0.8f;

            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("CharacterCurse") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("CharacterCurse") };
            UnityEngine.Debug.Log("[WWC][Card] Vampirism Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var LifeDrain = new GameObject("LifeDrain");
            vampirism = LifeDrain.gameObject.AddComponent<Vampirism_Mono>();
            statModifiers.AddObjectToPlayer = LifeDrain;
            statModifiers.GetAdditionalData().Vampire = true;
            //throw new NotImplementedException();
        }
        public override void OnRemoveCard()
        {
            Destroy(this.vampirism);
            //throw new NotImplementedException();
        }

        protected override string GetTitle()
        {
            return "Vampirism";
        }
        protected override string GetDescription()
        {
            return "Don't forget to suck blood!";
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
                    positive = false,
                    stat = "Curse",
                    amount = "Vampirism",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Life Steal",
                    amount = "+100%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Gravity",
                    amount = "-30%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Move Speed",
                    amount = "+15%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Jump Height",
                    amount = "+15%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Health",
                    amount = "-20%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return "WWC";
        }
    }
}
