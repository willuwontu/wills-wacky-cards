using HarmonyLib;
using UnityEngine;
using WWC.Cards.Curses;
using WWC.Extensions;
using WWC.MonoBehaviours;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(OutOfBoundsHandler))] 
    class OutOfBoundsHandler_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("RPCA_DisplayOutOfBoundsShield")]
        static void WrathDamage(OutOfBoundsHandler __instance, CharacterData ___data)
        {
            if (___data.currentCards.Contains(Wrath.card))
            {
                ___data.healthHandler.TakeDamage(51f * __instance.transform.up, ___data.transform.position, null, null, true);
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