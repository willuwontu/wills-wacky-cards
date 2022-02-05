using HarmonyLib;
using System.Collections.Generic;
using WWC.Cards;

namespace WWCC.Patches
{
    [HarmonyPatch(typeof(CurseEater))] 
    class CurseEater_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("CurseEaterAddClassStuff")]
        static bool ClassAdded(CurseEater __instance, CharacterStatModifiers characterStats)
        {
            ClassesManager.ClassesManager.Instance.OnClassCardSelect(characterStats, new List<string> { WillsWackyCardsClassCompat.CurseEaterClassName });
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CurseEaterRemoveClassStuff")]
        static bool ClassRemoved(CurseEater __instance)
        {
            return false;
        }
    }
}