using HarmonyLib;
using System.Collections.Generic;
using WWC.Cards;

namespace WWCC.Patches
{
    [HarmonyPatch(typeof(CurseEater))] 
    class CurseEater_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CurseEater.CurseEaterAddClassStuff))]
        static bool ClassAdded(CurseEater __instance, CharacterStatModifiers characterStats)
        {
            ClassesManager.ClassesManager.Instance.OnClassCardSelect(characterStats, new List<string> { WillsWackyCardsClassCompat.CurseEaterClassName });
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CurseEater.CurseEaterRemoveClassStuff))]
        static bool ClassRemoved(CurseEater __instance)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(Mechanic))]
    class Mechanic_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Mechanic.MechanicAddClassStuff))]
        static bool ClassAdded(CurseEater __instance, CharacterStatModifiers characterStats)
        {
            ClassesManager.ClassesManager.Instance.OnClassCardSelect(characterStats, new List<string> { WillsWackyCardsClassCompat.MechanicClassName });
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Mechanic.MechanicRemoveClassStuff))]
        static bool ClassRemoved(CurseEater __instance)
        {
            return false;
        }
    }
}