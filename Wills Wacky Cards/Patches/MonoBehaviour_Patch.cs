using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(MonoBehaviour))] 
    class MonoBehaviour_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MonoBehaviour.StopAllCoroutines))]
        static void DoTStopped(MonoBehaviour __instance)
        {
            if (__instance is DamageOverTime)
            {
                UnityEngine.Debug.Log("DOT Stop Coroutines Called");
            }
        }
    }
}