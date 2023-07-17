using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(SilenceHandler))] 
    class SilenceHandler_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("RPCA_AddSilence")]
        static void WillpowerSpeedUp(SilenceHandler __instance, CharacterData ___data, ref float f)
        {
            var data = ___data;
            if (f == 0f)
            {
                return;
            }
            if (!(data.stats.GetAdditionalData().willpower != 1f))
            {
                return;
            }

            if (data.stats.GetAdditionalData().willpower < 1f)
            {
                f *= (Mathf.Abs(data.stats.GetAdditionalData().willpower - 1f) + 1f);
            }
            else
            {
                f /= data.stats.GetAdditionalData().willpower;
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