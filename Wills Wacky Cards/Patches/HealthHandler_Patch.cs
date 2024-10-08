using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using WillsWackyManagers.Utils;
using WWC.Cards;
using UnboundLib;
using ModdingUtils.Utils;
using WWC.Cards.Curses;
using System;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(HealthHandler))]
    class HealthHandler_Patch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch("Heal")]
        static void NoHeal(Player ___player, ref float healAmount)
        {
            var noHeal = ___player.GetComponent<SavageWoundsDamage_Mono>();
            if (noHeal)
            {
                healAmount = 0f;
            }
        }

        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(methodName:"DoDamage")]
        [HarmonyPatch(methodName: "TakeDamage",argumentTypes: new Type[] { typeof(Vector2), typeof(Vector2), typeof(Color), typeof(GameObject), typeof(Player), typeof(bool), typeof(bool) })]
        static void WrathEffect(HealthHandler __instance, ref bool ignoreBlock, Player ___player)
        {
            if (___player.data.currentCards.Contains(Wrath.card))
            {
                ignoreBlock = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch("CallTakeDamage")]
        static void BleedEffect(HealthHandler __instance, Vector2 damage, Vector2 position, Player damagingPlayer, Player ___player)
        {
            if (!PlayerStatus.PlayerAliveAndSimulated(___player))
            {
                return;
            }
            var bleed = ___player.data.stats.GetAdditionalData().Bleed;
            if (bleed > 0f)
            {
                __instance.TakeDamageOverTime(damage * bleed, position, 3f - 0.5f/4f + bleed/4f, 0.25f, Color.red, null, damagingPlayer, true);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("DoDamage")]
        static void ApplyDamageReduction(HealthHandler __instance, ref Vector2 damage, Player ___player)
        {
            if (___player.data.stats.GetAdditionalData().DamageReduction > 0f)
            {
                var originalDamage = damage.magnitude;
                if (___player.data.stats.GetAdditionalData().DamageReduction < 1f)
                {
                    damage *= (1f - ___player.data.stats.GetAdditionalData().DamageReduction);

                    if (damage.magnitude < ___player.data.maxHealth * 0.05f && originalDamage > ___player.data.maxHealth * 0.05f)
                    {
                        damage = damage.normalized * ___player.data.maxHealth * 0.05f;
                    }
                }
                else
                {
                    if (damage.magnitude > ___player.data.maxHealth * 0.05f)
                    {
                        damage = damage.normalized * ___player.data.maxHealth * 0.05f;
                    }
                }
            }
        }

        //[HarmonyPrefix]
        //[HarmonyPatch("SomeMethod")]
        //static void MyMethodName()
        //{

        //}

        //[HarmonyPostfix]
        //[HarmonyPatch("SomeMethod")]
        //static void MyMethodName()
        //{

        //}
    }
}