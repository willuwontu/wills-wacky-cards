using System;
using System.Collections;
using System.Collections.Generic;
using UnboundLib.Cards;
using UnityEngine;
using Sirenix.OdinInspector;
using ClassesManagerReborn;

namespace WWC.UnityTools
{
    [Serializable]
    public class ClassCardRegistrationInfo
    {
        [AssetsOnly]
        [ShowInInspector]
        [SerializeField]
        public GameObject card;

        [EnumToggleButtons]
        [ShowInInspector]
        [SerializeField]
        public CardType cardType = CardType.Card;

        public int maxAllowed = 0;

        [EnumToggleButtons]
        [ShowInInspector]
        public RequirementsType requirementType = RequirementsType.Single;

        [AssetsOnly]
        [ShowIf("requirementType", RequirementsType.Single)]
        public GameObject singleReq;

        [AssetsOnly]
        [ShowIf("requirementType", RequirementsType.List)]
        public GameObject[] listReq;

        [AssetsOnly]
        [ShowIf("requirementType", RequirementsType.MultiList)]
        public CardReq[] multilistReq;
    }

    [Serializable]
    public enum RequirementsType
    {
        None,
        Single,
        List,
        MultiList
    }
}
