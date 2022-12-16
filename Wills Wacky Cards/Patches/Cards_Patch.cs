using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;
using Photon.Pun;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(ModdingUtils.Utils.Cards))]
    [HarmonyPriority(Priority.Last)]
    class Cards_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ModdingUtils.Utils.Cards.AddCardToPlayer), typeof(Player), typeof(CardInfo), typeof(bool), typeof(string), typeof(float), typeof(float), typeof(bool))]
        static void MomentumCorrection(Player player, ref CardInfo card, bool reassign = false, string twoLetterCode = "", float forceDisplay = 0f, float forceDisplayDelay = 0f, bool addToCardBar = true)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (!((card == WWC.Cards.ImmovableObject.card) || (card == WWC.Cards.UnstoppableForce.card)))
            {
                return;
            }

            UnboundLib.NetworkingManager.RPC_Others(typeof(CardChoice_Patch), nameof(CardChoice_Patch.URPCA_IncrementMomentum));
            MomentumTracker.stacks += 1;

            if (card == WWC.Cards.ImmovableObject.card)
            {
                card = MomentumTracker.GetDefensecard(1);
            }
            if (card == WWC.Cards.UnstoppableForce.card)
            {
                card = MomentumTracker.GetOffensecard(1);
            }

            MomentumTracker.stacks /= 4;
        }

        //[HarmonyPostfix]
        //static void OnlyOneMomentumCardWhenPicking(Player player, CardInfo card, ref bool __result)
        //{
        //    if (!player)
        //    {
        //        return;
        //    }

        //    if (!(CardChoice.instance.pickrID != -1))
        //    {
        //        return;
        //    }

        //    if (player.playerID != CardChoice.instance.pickrID)
        //    {
        //        return;
        //    }

        //    if (!((card.sourceCard == WWC.Cards.ImmovableObject.card) || (card.sourceCard == WWC.Cards.UnstoppableForce.card)))
        //    {
        //        return;
        //    }

        //    if (((List<GameObject>)CardChoice.instance.GetFieldValue("spawnedCards")).Count >= ((Transform[])CardChoice.instance.GetFieldValue("spawnedCards")).Length)
        //    {
        //        return;
        //    }

        //    if (card == WWC.Cards.ImmovableObject.card)
        //    {
        //        if (MomentumTracker.defenseFlag)
        //        {
        //            __result = false;
        //        }
        //    }

        //    if (card == WWC.Cards.UnstoppableForce.card)
        //    {
        //        if (MomentumTracker.offenseFlag)
        //        {
        //            __result = false;
        //        }
        //    }
        //}
    }
}