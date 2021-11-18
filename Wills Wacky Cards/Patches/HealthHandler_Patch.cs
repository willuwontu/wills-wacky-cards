using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using WillsWackyManagers.Utils;
using WWC.Cards;
using UnboundLib;

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

        [HarmonyPostfix]
        [HarmonyPatch("CallTakeDamage")]
        static void BleedEffect(HealthHandler __instance, Vector2 damage, Vector2 position, Player damagingPlayer, Player ___player)
        {
            var bleed = ___player.data.stats.GetAdditionalData().Bleed;
            if (bleed > 0f)
            {
                __instance.TakeDamageOverTime(damage * bleed, position, 3f - 0.5f/4f + bleed/4f, 0.25f, Color.red, null, damagingPlayer, true);
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