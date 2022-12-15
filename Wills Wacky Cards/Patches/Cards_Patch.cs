using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
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
            if (!(PhotonNetwork.IsMasterClient || PhotonNetwork.OfflineMode))
            {
                return;
            }

            if (!(card == WWC.Cards.ImmovableObject.card) || (card == WWC.Cards.UnstoppableForce.card))
            {
                return;
            }

            UnboundLib.NetworkingManager.RPC(typeof(CardChoice_Patch), nameof(CardChoice_Patch.URPCA_IncrementMomentum));
            var stacks = MomentumTracker.stacks+1;

            if (card == WWC.Cards.ImmovableObject.card)
            {
                card = MomentumTracker.GetDefensecard();
            }
            if (card == WWC.Cards.UnstoppableForce.card)
            {
                card = MomentumTracker.GetOffensecard();
            }
        }
    }
}