using HarmonyLib;
using UnityEngine;
using WillsWackyCards.Cards;
using WillsWackyCards.Extensions;
using WillsWackyCards.MonoBehaviours;
using UnboundLib;

namespace WillsWackyCards.Patches
{
    [HarmonyPatch(typeof(ModdingUtils.Utils.Cards))] 
    class Cards_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetRandomCardWithCondition")]
        static void CardChosen(CardInfo __result)
        {
            if (__result.cardName == "Momentum")
            {
                MomentumTracker.stacks += 1;
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