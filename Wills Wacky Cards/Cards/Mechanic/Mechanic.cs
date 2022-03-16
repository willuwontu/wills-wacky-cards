using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.Cards;
using WWC.Interfaces;
using WWC.MonoBehaviours;
using TMPro;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using UnboundLib.Utils;

namespace WWC.Cards
{
    public class Mechanic : CustomCard
    {
        public static CardInfo card = null;

        public const string MechanicClassName = "Mechanic";

        public static CardCategory MechanicClass = CustomCardCategories.instance.CardCategory("Mechanic");

        public static void MechanicAddClassStuff(CharacterStatModifiers characterStats)
        {
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.Add(CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == MechanicClass);
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Default"));
        }

        public static void MechanicRemoveClassStuff(CharacterStatModifiers characterStats)
        {
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.Add(MechanicClass);
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("Class"), CustomCardCategories.instance.CardCategory("NoRemove") };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.gameObject.AddComponent<Mechanic_Mono>();

            MechanicAddClassStuff(characterStats);

            var abyssalCard = CardManager.cards.Values.Select(card => card.cardInfo).First(c => c.name.Equals("AbyssalCountdown"));
            var statMods = abyssalCard.gameObject.GetComponentInChildren<CharacterStatModifiers>();
            var abyssalObj = statMods.AddObjectToPlayer;

            var mechObj = Instantiate(abyssalObj, player.transform);
            mechObj.name = "A_MechanicUpgrader";
            mechObj.transform.localPosition = Vector3.zero;

            var abyssal = mechObj.GetComponent<AbyssalCountdown>();

            var upgrader = mechObj.AddComponent<MechanicUpgrader>();
            upgrader.soundUpgradeChargeLoop = abyssal.soundAbyssalChargeLoop;
            upgrader.counter = 0;
            upgrader.upgradeTime = 6f;
            upgrader.timeToEmpty = 0.5f;
            upgrader.upgradeCooldown = 12f;
            upgrader.outerRing = abyssal.outerRing;
            upgrader.fill = abyssal.fill;
            upgrader.rotator = abyssal.rotator;
            upgrader.still = abyssal.still;
            upgrader.gunStatModifier.damage_mult = 1.4f;
            upgrader.characterDataModifier.maxHealth_mult = 1.4f;
            upgrader.characterDataModifier.health_mult = 1.4f;


            WillsWackyCards.instance.ExecuteAfterFrames(5, () => 
            {
                UnityEngine.GameObject.Destroy(abyssal);

                WillsWackyCards.instance.ExecuteAfterFrames(5, () =>
                {
                    var COs = mechObj.GetComponentsInChildren<Transform>().Where(child => child.parent == mechObj.transform).Select(child => child.gameObject).ToArray();

                    foreach (var CO in COs)
                    {
                        if (CO.transform != mechObj.transform.Find("Canvas"))
                        {
                            UnityEngine.GameObject.Destroy(CO);
                        }
                    }
                });
                upgrader.outerRing.color = new Color32(255, 167, 0, 255);
                upgrader.fill.color = new Color32(255, 196, 0, 10);
                upgrader.rotator.gameObject.GetComponentInChildren<ProceduralImage>().color = upgrader.outerRing.color;
                upgrader.still.gameObject.GetComponentInChildren<ProceduralImage>().color = upgrader.outerRing.color;
                mechObj.transform.Find("Canvas/Size/BackRing").GetComponent<ProceduralImage>().color = new Color32(200, 124, 33, 29);
            });

            var wobble = player.transform.Find("WobbleObjects");

            var upgradeFrame = Instantiate(player.transform.Find("WobbleObjects/Healthbar"), player.transform.Find("WobbleObjects")).gameObject;
            upgradeFrame.name = "Mechanic Level";
            upgradeFrame.transform.localScale = Vector3.one;
            upgradeFrame.transform.localPosition = new Vector3(0, 0.851f, 0);

            UnityEngine.GameObject.Destroy(upgradeFrame.GetComponent<HealthBar>());

            var upgradeCanvas = upgradeFrame.transform.Find("Canvas").gameObject;

            var COs = upgradeCanvas.GetComponentsInChildren<Transform>().Where(child => child.parent == upgradeCanvas.transform).Select(child => child.gameObject).ToArray();
            foreach (var CO in COs)
            {
                UnityEngine.GameObject.Destroy(CO);
            }

            RectTransform rect = null;

            var levelFrame = Instantiate<GameObject>(WillsWackyManagers.WillsWackyManagers.instance.WWWMAssets.LoadAsset<GameObject>("LevelFrame"), upgradeCanvas.transform);
            rect = levelFrame.GetComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.localPosition = new Vector3(-375, 0, 0);

            upgrader.levelFrame = upgradeFrame;
            upgrader.levelText = levelFrame.transform.Find("Ring/Level").gameObject.GetComponent<TextMeshProUGUI>();
            upgrader.levelText.text = $"{upgrader.upgradeLevel}";

            characterStats.objectsAddedToPlayer.Add(mechObj);

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            MechanicRemoveClassStuff(characterStats);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Mechanic";
        }
        protected override string GetDescription()
        {
            return "Scavenging parts from the area around them, Mechanics upgrade their abilities during battle.";
        }
        protected override GameObject GetCardArt()
        {
            GameObject art;

            try
            {
                art = WillsWackyManagers.WillsWackyManagers.instance.WWWMAssets.LoadAsset<GameObject>("C_Mechanic");
            }
            catch
            {
                art = null;
            }

            return art;
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
                    stat = "DMG per Upgrade",
                    amount = "+40%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "HP per Upgrade",
                    amount = "+40%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Upgrade Time",
                    amount = "6s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Upgrade Cooldown",
                    amount = "12s",
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

namespace WWC.MonoBehaviours
{
    [DisallowMultipleComponent]
    class Mechanic_Mono : ReversibleEffect, IPointStartHookHandler, IPlayerPickStartHookHandler, IGameStartHookHandler
    {
        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            this.SetLivesToEffect(int.MaxValue);
        }

        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName.ToLower() == "Mechanic".ToLower())
                {
                    haveCard = true;
                    break;
                }
            }

            if (!haveCard)
            {
                var classCards = data.currentCards.Where(card => card.categories.Contains(Mechanic.MechanicClass)).ToList();
                var cardIndeces = Enumerable.Range(0, player.data.currentCards.Count()).Where((index) => player.data.currentCards[index].categories.Contains(Mechanic.MechanicClass)).ToArray();
                if (classCards.Count() > 0)
                {
                    CardInfo[] replacePool = null;
                    if (classCards.Where(card => card.rarity == CardInfo.Rarity.Common).ToArray().Length > 0)
                    {
                        replacePool = classCards.Where(card => card.rarity == CardInfo.Rarity.Common).ToArray();
                    }
                    else if (classCards.Where(card => card.rarity == CardInfo.Rarity.Uncommon).ToArray().Length > 0)
                    {
                        replacePool = classCards.Where(card => card.rarity == CardInfo.Rarity.Uncommon).ToArray();
                    }
                    else if (classCards.Where(card => card.rarity == CardInfo.Rarity.Rare).ToArray().Length > 0)
                    {
                        replacePool = classCards.Where(card => card.rarity == CardInfo.Rarity.Rare).ToArray();
                    }
                    var replaced = replacePool[UnityEngine.Random.Range(0, replacePool.Length)];
                    classCards.Remove(replaced);
                    if (classCards.Count() > 1)
                    {
                        classCards.Shuffle();
                    }
                    classCards.Insert(0, Mechanic.card);

                    StartCoroutine(ReplaceCards(player, cardIndeces, classCards.ToArray()));
                }
                else
                {
                    UnityEngine.GameObject.Destroy(this);
                }
            }
        }

        private IEnumerator ReplaceCards(Player player, int[] indeces, CardInfo[] cards)
        {
            yield return ModdingUtils.Utils.Cards.instance.ReplaceCards(player, indeces, cards, null, true);

            yield break;
        }

        public void OnPlayerPickStart()
        {
            if (ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).blacklistedCategories.Contains(WWC.Cards.Mechanic.MechanicClass))
            {
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).blacklistedCategories.RemoveAll((category) => category == WWC.Cards.Mechanic.MechanicClass);
            }


            CheckIfValid();
        }

        public void OnPointStart()
        {
            CheckIfValid();
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnOnDestroy()
        {
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).blacklistedCategories.Add(WWC.Cards.Mechanic.MechanicClass);
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }
    }
}