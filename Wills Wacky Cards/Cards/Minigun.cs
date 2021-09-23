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
    class Minigun : CustomCard
    {
        private static Minigun_Mono minigun;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            var block = cardInfo.gameObject.GetOrAddComponent<Block>();
            gun.attackSpeed = 1f/2000f;
            gun.timeBetweenBullets = 0.05f;
            gun.projectileSpeed = 3f;
            gun.destroyBulletAfter = 0.2f;
            gun.spread = 0.2f;
            gun.knockback = .05f;
            gun.reloadTime *= 50f;
            gun.reloadTimeAdd += 15f;

            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("WWC Gun Type") };
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("No Minigun") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("No Minigun") };
            UnityEngine.Debug.Log("[WWC][Card] Minigun Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var heatBar = Instantiate(player.transform.Find("WobbleObjects/Healthbar"), player.transform.Find("WobbleObjects"));
            heatBar.name = "HeatBar";
            heatBar.Translate(new Vector3(.95f, -1.1f, 0));
            heatBar.localScale.Set(0.5f, 1f, 1f);
            heatBar.localScale = new Vector3(0.6f, 1.4f, 1f);
            heatBar.Rotate(0f, 0f, 90f);
            minigun = player.gameObject.AddComponent<Minigun_Mono>();
        }
        public override void OnRemoveCard()
        {

        }

        protected override string GetTitle()
        {
            return "Minigun";
        }
        protected override string GetDescription()
        {
            return "Minigun goes brrrrrr";
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
                    stat = "Ammo",
                    amount = "Infinite",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Attack Speed",
                    amount = "+2000%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet Speed",
                    amount = "+200%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Damage",
                    amount = "-97%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Decreased",
                    amount = "Range",
                    simepleAmount = CardInfoStat.SimpleAmount.lower
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }

        public override string GetModName()
        {
            return "WWC";
        }
    }
}
