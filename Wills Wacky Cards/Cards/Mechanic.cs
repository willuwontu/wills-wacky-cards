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
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

namespace WWC.Cards
{
    public class Mechanic : CustomCard
    {
        public const string MechanicClassName = "Mechanic";

        public static CardCategory MechanicClass = CustomCardCategories.instance.CardCategory("Mechanic");

        public static void MechanicAddClassStuff(CharacterStatModifiers characterStats)
        {
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.Add(CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == MechanicClass);
        }

        public static void MechanicRemoveClassStuff(CharacterStatModifiers characterStats)
        {
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.Add(MechanicClass);
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var abyssalCard = CardChoice.instance.cards.First(c => c.name.Equals("AbyssalCountdown"));
            var statMods = abyssalCard.gameObject.GetComponentInChildren<CharacterStatModifiers>();
            var abyssalObj = statMods.AddObjectToPlayer;

            var mechObj = Instantiate(abyssalObj, player.transform);
            mechObj.name = "A_MechanicUpgrader";
            mechObj.transform.localPosition = Vector3.zero;

            var abyssal = mechObj.GetComponent<AbyssalCountdown>();

            var upgrader = mechObj.AddComponent<MechanicUpgrader>();
            upgrader.soundUpgradeChargeLoop = abyssal.soundAbyssalChargeLoop;
            upgrader.counter = 0;
            upgrader.timeToFill = 5f;
            upgrader.timeToEmpty = 0.5f;
            upgrader.outerRing = abyssal.outerRing;
            upgrader.fill = abyssal.fill;
            upgrader.rotator = abyssal.rotator;
            upgrader.still = abyssal.still;

            WillsWackyCards.instance.ExecuteAfterFrames(5, () => 
            {
                UnityEngine.GameObject.Destroy(abyssal);

                var COs = mechObj.GetComponentsInChildren<Transform>().Where(child => child.parent == mechObj.transform).Select(child => child.gameObject).ToArray();

                foreach (var CO in COs)
                {
                    if (!(CO.transform.gameObject == mechObj.transform.Find("Canvas").gameObject) && !(upgrader.upgradeObjects.Contains(CO)))
                    {
                        UnityEngine.GameObject.Destroy(CO);
                    }
                }
                upgrader.outerRing.color = new Color32(255, 167, 0, 255);
                upgrader.fill.color = new Color32(255, 196, 0, 10);
                upgrader.rotator.gameObject.GetComponentInChildren<ProceduralImage>().color = upgrader.outerRing.color;
                upgrader.still.gameObject.GetComponentInChildren<ProceduralImage>().color = upgrader.outerRing.color;
                mechObj.transform.Find("Canvas/Size/BackRing").GetComponent<ProceduralImage>().color = new Color32(200, 124, 33, 29);
            });

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mechObj = player.gameObject.transform.Find("A_MechanicUpgrader");
            UnityEngine.GameObject.Destroy(mechObj);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Mechanic";
        }
        protected override string GetDescription()
        {
            return "While standing still, upgrade yourself.";
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
                    stat = "Effect",
                    amount = "No",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.TechWhite;
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