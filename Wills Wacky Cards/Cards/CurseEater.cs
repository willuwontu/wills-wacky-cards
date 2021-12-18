using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using WillsWackyManagers.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    class CurseEater : CustomCard
    {
        internal static CardCategory CurseEaterClass = CustomCardCategories.instance.CardCategory("Curse Eater");
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseSpawnerCategory, CustomCardCategories.instance.CardCategory("Class") };
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.Add(CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == CurseEaterClass);

            player.gameObject.GetOrAddComponent<CurseEater_Mono>();

            WillsWackyCards.instance.ExecuteAfterFrames(20, () => CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); }));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == CustomCardCategories.instance.CardCategory("Class"));
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.Add(CurseEaterClass);

            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
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
    class CurseEater_Mono : Hooked_Mono
    {
        private float multiplier = 0f;
        private bool increased = false;

        private CharacterData data;
        private Player player;
        private CharacterStatModifiers stats;

        private void Start()
        {
            HookedMonoManager.instance.hookedMonos.Add(this);
            data = GetComponentInParent<CharacterData>();
        }

        private void Update()
        {
            if (!player)
            {
                if (!(data is null))
                {
                    player = data.player;
                    stats = data.stats;
                }

            }
        }

        public override void OnPointStart()
        {
            if (!increased)
            {
                increased = true;
                var curses = CurseManager.instance.GetAllCursesOnPlayer(player).Count();
                multiplier = 0.1f * curses;
                stats.lifeSteal += multiplier;
            }
        }

        public override void OnPointEnd()
        {
            if (increased)
            {
                stats.lifeSteal -= multiplier;
                increased = false;
            }
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            OnPointEnd();
            HookedMonoManager.instance.hookedMonos.Remove(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}