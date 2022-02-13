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
using WWC.MonoBehaviours;
using WWC.Interfaces;
using WillsWackyManagers.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    public class CurseEater : CustomCard
    {
        public static CardInfo card = null;

        public const string CurseEaterClassName = "Curse Eater";

        public static CardCategory CurseEaterClass = CustomCardCategories.instance.CardCategory("Curse Eater");

        public static void CurseEaterAddClassStuff(CharacterStatModifiers characterStats)
        {
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.Add(CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == CurseEaterClass);
        }

        public static void CurseEaterRemoveClassStuff(CharacterStatModifiers characterStats)
        {
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.Add(CurseEaterClass);
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseSpawnerCategory, CustomCardCategories.instance.CardCategory("Class") };
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            CurseEaterAddClassStuff(characterStats);

            var mono = player.gameObject.GetOrAddComponent<CurseEater_Mono>();

            WillsWackyCards.instance.ExecuteAfterFrames(20, () => CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); }));
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            CurseEaterRemoveClassStuff(characterStats);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Curse Eater";
        }
        protected override string GetDescription()
        {
            return "A curse eater learns to turn curses into an advantage.";
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
                    stat = "Lifesteal",
                    amount = "+80%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Lifesteal per Curse",
                    amount = "+10%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Curse",
                    amount = "+1",
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

namespace WWC.MonoBehaviours
{
    [DisallowMultipleComponent]
    class CurseEater_Mono : ReversibleEffect, IPointEndHookHandler, IPointStartHookHandler, IPlayerPickStartHookHandler, IGameStartHookHandler
    {
        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
        }

        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName.ToLower() == "Curse Eater".ToLower())
                {
                    haveCard = true;
                    break;
                }
            }

            if (!haveCard)
            {
                var classCards = data.currentCards.Where(card => card.categories.Contains(CurseEater.CurseEaterClass)).ToList();
                var cardIndeces = Enumerable.Range(0, player.data.currentCards.Count()).Where((index) => player.data.currentCards[index].categories.Contains(CurseEater.CurseEaterClass)).ToArray();
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
                    classCards.Insert(0, CurseEater.card);

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
            if (ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).blacklistedCategories.Contains(WWC.Cards.CurseEater.CurseEaterClass))
            {
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).blacklistedCategories.RemoveAll((category) => category == WWC.Cards.CurseEater.CurseEaterClass);
            }


            CheckIfValid();
        }

        public void OnPointStart()
        {
            var curses = CurseManager.instance.GetAllCursesOnPlayer(player).Count();
            var multiplier = 0.1f * curses + 0.8f;
            characterStatModifiersModifier.lifeSteal_add += multiplier;
            ApplyModifiers();

            CheckIfValid();
        }

        public void OnPointEnd()
        {
            ClearModifiers();
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnOnDestroy()
        {
            OnPointEnd();
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(stats).blacklistedCategories.Add(WWC.Cards.CurseEater.CurseEaterClass);
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
        }
    }
}