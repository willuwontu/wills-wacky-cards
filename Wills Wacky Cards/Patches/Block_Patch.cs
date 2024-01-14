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
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Block.IsBlocking))]
        static void IncreasedBlockTime(Block __instance, ref bool __result, CharacterData ___data)
        {

            if (__instance.sinceBlock < (0.3 + ___data.stats.GetAdditionalData().extraBlockTime))
            {
                __instance.ShowStatusEffectBlock();
            }

            __result = __result || __instance.sinceBlock < (0.3 + ___data.stats.GetAdditionalData().extraBlockTime);
        }

        [HarmonyPrefix]
        [HarmonyPriority(int.MaxValue)]
        [HarmonyPatch("RPCA_DoBlock")]
        static bool StoppedByStunAndSilence(Block __instance, CharacterData ___data)
        {
            if (___data.view.IsMine && (___data.isStunned || ___data.isSilenced))
            {
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPriority(int.MaxValue)]
        [HarmonyPatch("TryBlock")]
        static bool TryStoppedByStunAndSilence(Block __instance, CharacterData ___data)
        {
            if (___data.view.IsMine && (___data.isStunned || ___data.isSilenced))
            {
                return false;
            }

            return true;
        }

        //[HarmonyPostfix]
        //[HarmonyPriority(int.MinValue)]
        //[HarmonyPatch("Cooldown")]
        //static void OnCooldownWhileSilencedAndStunned(Block __instance, CharacterData ___data, ref float __result)
        //{
        //    if (___data.view.IsMine && (___data.isStunned || ___data.isSilenced))
        //    {
        //        __result = float.MaxValue;
        //    }
        //}

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