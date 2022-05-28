using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch()]
    class Egg_Patch
    {
        static void EggRarity(ref CardInfo.Rarity __result)
        {
            __result = RarityLib.Utils.RarityUtils.GetRarity("E G G");
        }
    }
}