using HarmonyLib;
using UnityEngine;
using WillsWackyCards.Extensions;
using WillsWackyCards.MonoBehaviours;
using UnboundLib;

namespace WillsWackyCards.Patches
{
    [HarmonyPatch(typeof(HealthHandler))] 
    class HealthHandler_Patch
    {
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