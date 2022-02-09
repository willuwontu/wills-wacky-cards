using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(Block))] 
    class Block_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Block.IsBlocking))]
        static bool IncreasedBlockTime(Block __instance, ref bool __result, CharacterData ___data)
        {

            if (__instance.sinceBlock < (0.3 + ___data.stats.GetAdditionalData().extraBlockTime))
            {
                __instance.ShowStatusEffectBlock();
            }

            __result = __instance.sinceBlock < (0.3 + ___data.stats.GetAdditionalData().extraBlockTime);

            return false;
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