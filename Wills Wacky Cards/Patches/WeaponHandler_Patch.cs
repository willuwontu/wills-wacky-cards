using HarmonyLib;
using UnityEngine;
using WillsWackyCards.Extensions;
using WillsWackyCards.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WillsWackyCards.Patches
{
    [HarmonyPatch(typeof(WeaponHandler))] 
    class WeaponHandler_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Attack")]
        static bool ChargeAttack(WeaponHandler __instance)
        {
            var weaponHandler = __instance;
            if (weaponHandler.gun.useCharge)
            {
                return false;
            }
            return true;
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