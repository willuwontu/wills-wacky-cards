using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;
using WWC.UI;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(Player))] 
    class Player_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void Spotlight(Player __instance)
        {
            if (__instance.data.view.IsMine)
            {
                PlayerSpotlight.AddSpotToPlayer(__instance);
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