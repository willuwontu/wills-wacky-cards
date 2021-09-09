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
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.attackSpeed = 1f/2000f;
            gun.timeBetweenBullets = 0.1f;
            gun.projectileSpeed = 3f;
            gun.destroyBulletAfter = 0.15f;
            gun.spread = 0.15f;
            gun.knockback = .01f;
            gun.reloadTime *= 50f;
            gun.reloadTimeAdd += 15f;

            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("GunType") };
            UnityEngine.Debug.Log("[WWC][Card] Minigun Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var heatBar = Instantiate(player.transform.Find("WobbleObjects/Healthbar"), player.transform.Find("WobbleObjects"));
            heatBar.name = "HeatBar";
            heatBar.Translate(new Vector3(0.9f, -1f, 0));
            heatBar.localScale.Set(0.5f, 1f, 1f);
            heatBar.localScale = new Vector3(0.6f, 1.25f, 1f);
            heatBar.Rotate(0f, 0f, 90f);
            var heat = player.gameObject.AddComponent<Minigun_Mono>();
            //throw new NotImplementedException();
        }
        public override void OnRemoveCard()
        {
            //throw new NotImplementedException();
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
