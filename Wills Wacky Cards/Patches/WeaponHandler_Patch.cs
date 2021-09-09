using HarmonyLib;
using UnityEngine;
using WillsWackyCards.Extensions;

namespace WillsWackyCards.Patches
{
    [HarmonyPatch(typeof(WeaponHandler))]
    class WeaponHandler_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Attack")]
        static bool MinigunOverheat(WeaponHandler __instance, Gun ___gun)
        {
            //if (___gun.GetAdditionalData().overHeated)
            //{
            //    return false;
            //}
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