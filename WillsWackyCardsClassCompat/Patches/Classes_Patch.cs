using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using WWC.Cards;
using UnboundLib;

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
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == CurseEater.CurseEaterClass);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CurseEater.CurseEaterRemoveClassStuff))]
        static bool ClassRemoved(CurseEater __instance)
        {
            return true;
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
            ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.RemoveAll((category) => category == Mechanic.MechanicClass);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Mechanic.MechanicRemoveClassStuff))]
        static bool ClassRemoved(CurseEater __instance)
        {
            return true;
        }
    }

    //[HarmonyPatch(typeof(ClassesManager.ClassesManager))]
    //class ClassesManager_Patch
    //{
    //    [HarmonyPrefix]
    //    [HarmonyPatch(nameof(ClassesManager.ClassesManager.OnClassCardSelect))]
    //    static void GetPrevState(ClassesManager.ClassesManager __instance, CharacterStatModifiers characterStats, List<string> progressionCategoryNames, ref CheckedValues __state)
    //    {
    //        __state = new CheckedValues(progressionCategoryNames.Select(name => name.ToLower()).ToArray(), ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.Where(category => category != null && category.name != null).Select(category => category.name.ToLower()).ToArray());
    //    }

    //    [HarmonyPostfix]
    //    [HarmonyPatch(nameof(ClassesManager.ClassesManager.OnClassCardSelect))]
    //    static void GetAfterState(ClassesManager.ClassesManager __instance, CharacterStatModifiers characterStats, List<string> progressionCategoryNames, CheckedValues __state)
    //    {
    //        var updatedBlacklist = ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterStats).blacklistedCategories.Where(category => category != null && category.name != null).Select(category => category.name.ToLower()).ToArray();
    //        UnityEngine.Debug.Log($"ClassesManager.OnClassCardSelect() run:\nSelected Category Names:\n{string.Join("\n", __state.addedCategories)}\n\nOriginal Blacklist:\n\n{string.Join("\n", __state.originalBlacklist)}\n\nNew Blacklist:\n\n{string.Join("\n", updatedBlacklist)}\n\nAll Class Progression Categories:\n{string.Join("\n", ClassesManager.ClassesManager.Instance.ClassProgressionCategories.Keys.Select(str => str.ToLower()).ToArray())}");
    //    }
    //}

    //class CheckedValues
    //{
    //    public string[] addedCategories;
    //    public string[] originalBlacklist;

    //    public CheckedValues()
    //    {

    //    }

    //    public CheckedValues(string[] added, string[] original)
    //    {
    //        addedCategories = added;
    //        originalBlacklist = original;
    //    }
    //}
}