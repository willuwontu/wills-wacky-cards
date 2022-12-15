using HarmonyLib;
using UnityEngine;
using Sonigon;
using WWC.MonoBehaviours;
using WWC.Extensions;
using UnboundLib;
using UnboundLib.Networking;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(CardChoice))] 
    class CardChoice_Patch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch("SpawnUniqueCard")]
        static void MomentumReplace(CardChoice __instance, ref GameObject __result, Vector3 pos, Quaternion rot)
        {
            var card = __result.GetComponent<CardInfo>();

            if ((card.sourceCard == WWC.Cards.ImmovableObject.card) || (card.sourceCard == WWC.Cards.UnstoppableForce.card))
            {
                if (PlayerManager.instance.GetPlayerWithID(__instance.pickrID).data.view.IsMine)
                {
                    UnboundLib.NetworkingManager.RPC_Others(typeof(CardChoice_Patch), nameof(CardChoice_Patch.URPCA_IncrementMomentum));
                    MomentumTracker.stacks += 1;
                }

                if (card.sourceCard == WWC.Cards.ImmovableObject.card)
                {
                    var temp = __result;
                    WillsWackyCards.instance.ExecuteAfterFrames(5, () =>
                    {
                        Photon.Pun.PhotonNetwork.Destroy(temp);
                    });

                    var momentumCard = MomentumTracker.GetDefensecard(0);
                    __result = (GameObject)__instance.InvokeMethod("Spawn", new object[] { momentumCard.gameObject, pos, rot });
                    __result.GetComponent<CardInfo>().sourceCard = momentumCard;
                    __result.GetComponentInChildren<DamagableEvent>().GetComponent<Collider2D>().enabled = false;
                    __result.name = WWC.Cards.ImmovableObject.card.gameObject.name;
                    MomentumTracker.defenseFlag = true;
                }

                if (card.sourceCard == WWC.Cards.UnstoppableForce.card)
                {
                    var temp = __result;
                    WillsWackyCards.instance.ExecuteAfterFrames(5, () =>
                    {
                        Photon.Pun.PhotonNetwork.Destroy(temp);
                    });

                    var momentumCard = MomentumTracker.GetOffensecard(0);
                    __result = (GameObject)__instance.InvokeMethod("Spawn", new object[] { momentumCard.gameObject, pos, rot });
                    __result.GetComponent<CardInfo>().sourceCard = momentumCard;
                    __result.GetComponentInChildren<DamagableEvent>().GetComponent<Collider2D>().enabled = false;
                    __result.name = WWC.Cards.UnstoppableForce.card.gameObject.name;
                    MomentumTracker.offenseFlag = true;
                }
            }
        }

        [UnboundRPC]
        internal static void URPCA_IncrementMomentum()
        {
            WWC.MonoBehaviours.MomentumTracker.stacks += 1;
            //UnityEngine.Debug.Log($"Stacks increased to {WWC.MonoBehaviours.MomentumTracker.stacks}");
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