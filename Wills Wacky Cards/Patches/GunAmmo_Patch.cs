using HarmonyLib;
using UnityEngine;
using WillsWackyCards.Extensions;
using WillsWackyCards.MonoBehaviours;
using UnboundLib;

namespace WillsWackyCards.Patches
{
    [HarmonyPatch(typeof(GunAmmo))] 
    class GunAmmo_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ReloadTime")]
        static void ChangedReloadTime(ref float __result, Gun ___gun)
        {
            var gun = ___gun;
            if (gun.GetAdditionalData().useMinimumReloadSpeed && __result < gun.GetAdditionalData().minimumReloadSpeed)
            {
                //UnityEngine.Debug.Log(string.Format("[WWC][GunAmmo_Patch][Minimum] Reload time changed from {0} to {1} extra.", __result, gun.GetAdditionalData().minimumReloadSpeed));
                __result = gun.GetAdditionalData().minimumReloadSpeed;
            }

            if (gun.GetAdditionalData().useForcedReloadSpeed && __result != gun.GetAdditionalData().forcedReloadSpeed)
            {
                //UnityEngine.Debug.Log(string.Format("[WWC][GunAmmo_Patch][Forced] Reload time changed from {0} to {1} extra.", __result, gun.GetAdditionalData().forcedReloadSpeed));
                __result = gun.GetAdditionalData().forcedReloadSpeed;
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