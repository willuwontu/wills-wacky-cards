using HarmonyLib;
using UnityEngine;
using Sonigon;
using WWC.MonoBehaviours;
using WWC.Extensions;
using UnboundLib;
using UnboundLib.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using WWC.Cards.Curses;
using Photon.Pun;
using System.Linq;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(CardChoice))] 
    class CardChoice_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CardChoice.StartPick))]
        static void FutureSightNulls(int pickerIDToSet)
        {
            Player player = PlayerManager.instance.GetPlayerWithID(pickerIDToSet);

            if (player != null)
            {
                if (player.data.currentCards.Contains(WWC.Cards.FutureSight.card))
                {
                    Nullmanager.CharacterStatModifiersExtension.AjustNulls(player.data.stats, 4 * player.data.currentCards.Where(c => c == WWC.Cards.FutureSight.card).Count());
                }
            }
        }

        class SimpleEnumerator : IEnumerable
        {
            public IEnumerator enumerator;
            public Action prefixAction, postfixAction;
            public Action<object> preItemAction, postItemAction;
            public Func<object, object> itemAction;
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
            public IEnumerator GetEnumerator()
            {
                prefixAction();
                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    preItemAction(item);
                    yield return itemAction(item);
                    postItemAction(item);
                }
                postfixAction();
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch("ReplaceCards")]
        static void GetSpawnedCards(CardChoice __instance, List<GameObject> ___spawnedCards, ref IEnumerator __result)
        {
            Action prefixAction = () => { };
            Action<object> preItemAction = (item) => {  };
            Action<object> postItemAction = (item) => {  };
            Func<object, object> itemAction = (item) => { return item; };

            Action postfixAction = () =>
            {
                List<string> cardNames = new List<string>();
                foreach (var cardObj in ___spawnedCards)
                {
                    var card = cardObj.GetComponent<CardInfo>();
                    if (card)
                    {
                        cardNames.Add(cardObj.name.Replace("(Clone)",""));
                        //UnityEngine.Debug.Log(cardObj.name.Replace("(Clone)", ""));
                    }
                }

                UnboundLib.NetworkingManager.RPC(typeof(MissedOpportunities), nameof(MissedOpportunities.URPCA_CardsSeen), new object[] { __instance.pickrID, cardNames.ToArray() });
            };

            var myEnumerator = new SimpleEnumerator()
            {
                enumerator = __result,
                prefixAction = prefixAction,
                postfixAction = postfixAction,
                preItemAction = preItemAction,
                postItemAction = postItemAction,
                itemAction = itemAction
            };
            __result = myEnumerator.GetEnumerator();
        }

        [HarmonyPostfix]
        [HarmonyPatch("IDoEndPick")]
        static void GetPickedCard(CardChoice __instance, GameObject pickedCard, ref IEnumerator __result)
        {
            Action prefixAction = () =>
            {
                if (!YardSale.yardSaleActive)
                {
                    return;
                }

                if (pickedCard != null && pickedCard.GetComponent<CardInfo>() & (PlayerManager.instance.GetPlayerWithID(CardChoice.instance.pickrID).data.view.IsMine || PhotonNetwork.OfflineMode))
                {
                    //UnityEngine.Debug.Log("Sending URPCA");
                    UnboundLib.NetworkingManager.RPC(typeof(YardSale), nameof(YardSale.URPCA_AddToListOfCardsToRemove), new object[] { YardSale.yardSalePlayer.playerID, pickedCard.name.Replace("(Clone)", "") });
                }
            };
            Action<object> preItemAction = (item) => { };
            Action<object> postItemAction = (item) => { };
            Func<object, object> itemAction = (item) => { return item; };

            Action postfixAction = () =>
            {
            };

            var myEnumerator = new SimpleEnumerator()
            {
                enumerator = __result,
                prefixAction = prefixAction,
                postfixAction = postfixAction,
                preItemAction = preItemAction,
                postItemAction = postItemAction,
                itemAction = itemAction
            };
            __result = myEnumerator.GetEnumerator();
        }

        [HarmonyPostfix]
        [HarmonyPatch("ReplaceCards")]
        static void OnlyModifyCardsGeneratedDuringPick(CardChoice __instance, GameObject pickedCard, ref IEnumerator __result)
        {
            Action prefixAction = () => {
                if (!YardSale.yardSaleActive)
                {
                    return;
                }
                YardSale.generatingCardsDuringYardSale = true;
            };
            Action<object> preItemAction = (item) => { };
            Action<object> postItemAction = (item) => { };
            Func<object, object> itemAction = (item) => { return item; };

            Action postfixAction = () => {
                if (!YardSale.yardSaleActive)
                {
                    return;
                }
                YardSale.generatingCardsDuringYardSale = false;
            };

            var myEnumerator = new SimpleEnumerator()
            {
                enumerator = __result,
                prefixAction = prefixAction,
                postfixAction = postfixAction,
                preItemAction = preItemAction,
                postItemAction = postItemAction,
                itemAction = itemAction
            };
            __result = myEnumerator.GetEnumerator();
        }

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