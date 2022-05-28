using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(CharacterStatModifiers))] 
    class CharacterStatModifiers_Patch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch("ConfigureMassAndSize")]
        static void MassAdjustment(CharacterStatModifiers __instance, CharacterData ___data)
        {
            if (__instance.GetAdditionalData().MassModifier != 1f)
            {
                float massCurr = (float)___data.playerVel.GetFieldValue("mass");
                float massMod = __instance.GetAdditionalData().MassModifier;
                float massTarg = massCurr * massMod;
                ___data.playerVel.SetFieldValue("mass", massTarg);
            }

            __instance.transform.root.localScale = new Vector3(Mathf.Clamp(__instance.transform.localScale.x, 0.5f, 15f), Mathf.Clamp(__instance.transform.localScale.y, 0.5f, 15f), 1.2f);
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