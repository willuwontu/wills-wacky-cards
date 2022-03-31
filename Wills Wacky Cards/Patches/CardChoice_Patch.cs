using HarmonyLib;
using UnityEngine;
using Sonigon;
using WWC.MonoBehaviours;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(CardChoice))] 
    class CardChoice_Patch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch("SpawnUniqueCard")]
        static void MomentumReplace(CardChoice __instance, ref GameObject __result, Vector3 pos, Quaternion rot)
        {
            var card = __result.GetComponent<CardInfo>();

            if ((card.sourceCard == WWC.Cards.ImmovableObject.card) || (card.sourceCard == WWC.Cards.UnstoppableForce.card))
            {
                WWC.MonoBehaviours.MomentumTracker.stacks += 1;
                if (card.sourceCard == WWC.Cards.ImmovableObject.card)
                {
                    UnityEngine.GameObject.Destroy(__result);

                    var stacks = MomentumTracker.stacks;
                    __result = (GameObject)__instance.InvokeMethod("Spawn", new object[] { MomentumTracker.createdDefenseCards[stacks].gameObject, pos, rot });
                    __result.GetComponent<CardInfo>().sourceCard = MomentumTracker.createdDefenseCards[stacks];
                    __result.GetComponentInChildren<DamagableEvent>().GetComponent<Collider2D>().enabled = false;
                }

                if (card.sourceCard == WWC.Cards.UnstoppableForce.card)
                {
                    UnityEngine.GameObject.Destroy(__result);

                    var stacks = MomentumTracker.stacks;
                    __result = (GameObject)__instance.InvokeMethod("Spawn", new object[] { MomentumTracker.createdOffenseCards[stacks].gameObject, pos, rot });
                    __result.GetComponent<CardInfo>().sourceCard = MomentumTracker.createdOffenseCards[stacks];
                    __result.GetComponentInChildren<DamagableEvent>().GetComponent<Collider2D>().enabled = false;
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