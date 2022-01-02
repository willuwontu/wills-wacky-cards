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
using HarmonyLib;

namespace WWC.Cards
{
    class BoundByBlood : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<BoundByBlood_Mono>();
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<BoundByBlood_Mono>();
            UnityEngine.GameObject.Destroy(mono);
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Bound by Blood";
        }
        protected override string GetDescription()
        {
            return "When you take damage while blocking, the damage dealer recieves it too.";
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
    public class BoundByBlood_Mono : Hooked_Mono
    {
        private CharacterData data;
        private Player player;

        private void Start()
        {
            HookedMonoManager.instance.hookedMonos.Add(this);
            data = GetComponentInParent<CharacterData>();
            player = data.player;
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            HookedMonoManager.instance.hookedMonos.Remove(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}

namespace WWC.Patches
{
    [HarmonyPatch(typeof(HealthHandler))]
    class BoundByBloodHealthHandler_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("DoDamage")]
        static void BindDamage(HealthHandler __instance, Player damagingPlayer, Vector2 damage, Player ___player)
        {
            var player = ___player;
            var bound = player.gameObject.GetComponent<BoundByBlood_Mono>();

            if (bound)
            {
                if (damagingPlayer)
                {
                    if (damagingPlayer != player && player.data.block.IsBlocking())
                    {
                        damagingPlayer.data.healthHandler.DoDamage(damage, player.transform.position, Color.red, null, player, false, true, true);
                    }
                }
            }
        }
    }

    //[HarmonyPatch(typeof(Block))]
    //class BoundByBloodBlock_Patch
    //{
    //    [HarmonyPostfix]
    //    [HarmonyPatch("Cooldown")]
    //    static void BindDamage(Block __instance, ref float __result, CharacterData ___data)
    //    {
    //        var player = ___data.player;
    //        var bound = player.gameObject.GetComponent<BoundByBlood_Mono>();

    //        if (bound)
    //        {
    //            if (__result < 3f)
    //            {
    //                __result = 3f;
    //            }
    //        }
    //    }
    //}
}