using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WWC.Patches
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