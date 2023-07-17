using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(StunHandler))] 
    class StunHandler_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AddStun")]
        static void WillpowerSpeedUp(StunHandler __instance, CharacterData ___data, ref float f)
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

            if (data.stats.GetAdditionalData().willpower < 1f && data.block.IsBlocking())
            {
                if (f > data.stunTime)
                {
                    data.stunTime = f;
                }
                if (!data.isStunned)
                {
                    __instance.InvokeMethod("StartStun", new object[] { });
                }
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