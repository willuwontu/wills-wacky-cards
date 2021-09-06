using HarmonyLib;
using UnityEngine;
using WillsWackyCards.Extensions;

namespace WillsWackyCards.Patches
{
    [HarmonyPatch(typeof(GunAmmo))] // Patching the getShootRotation method in the gun object.
    class GunAmmo_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Shoot")]
        static bool MinigunMadness(GunAmmo __instance)
        {
            Gun gun = __instance.GetComponentInParent<Gun>();
            if (gun.GetAdditionalData().minigun)
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