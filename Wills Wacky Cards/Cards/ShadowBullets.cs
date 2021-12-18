using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WillsWackyManagers.Utils;
using WWC.Extensions;
using WWC.MonoBehaviours;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    class ShadowBullets : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            var drillCard = UnboundLib.Utils.CardManager.cards.Values.Where((card) => card.cardInfo.cardName.ToLower() == "Drill Ammo".ToLower()).Select((card) => card.cardInfo).FirstOrDefault();

            var drillGun = drillCard.gameObject.GetComponent<Gun>();

            GameObject drill = null;

            drill = GameObject.Find("A_ShadowBullet");

            if (!drill)
            {
                drill = Instantiate(drillGun.objectsToSpawn[0].AddToProjectile);
                drill.name = "A_ShadowBullet";

                var drillRay = drill.GetComponent<RayHitDrill>();

                drillRay.metersOfDrilling = 5000f;
                drillRay.speedModFlat = 2f;
                drillRay.speedMod = 0.1f; 
            }

            gun.objectsToSpawn = new ObjectsToSpawn[] { new ObjectsToSpawn { AddToProjectile = drill } };

            gun.unblockable = true;
            //gun.projectileColor = new Color(46f / 255f, 46f / 255f, 46f / 255f, 0.3f);
            gun.bulletDamageMultiplier = 0.75f;
            gun.reloadTimeAdd = 0.5f;
            gun.attackSpeed = 1.3f;
            gun.gravity = 0f;

            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseSpawnerCategory, CurseEater.CurseEaterClass, CustomCardCategories.instance.CardCategory("Shadow Bullets") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("Corrupted Ammunition") };

            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.projectileColor = new Color(10f / 255f, 10f / 255f, 10f / 255f, 0.5f);
            //WillsWackyCards.instance.ExecuteAfterFrames(20, () => CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); }));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Shadow Bullets";
        }
        protected override string GetDescription()
        {
            return "";
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
                    stat = "Walls",
                    amount = "Pierce",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullets",
                    amount = "Unblockable",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Damage",
                    amount = string.Format("-{0:F0}%", ((1f / 0.75f) - 1f) * 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Attack Speed",
                    amount = "-30%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload Time",
                    amount = "+0.5s",
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
            return WillsWackyCards.ModInitials;
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
