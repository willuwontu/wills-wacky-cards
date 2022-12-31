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
    public class CardCardList
    {
        [AssetsOnly]
        [ShowInInspector]
        [SerializeField]
        public GameObject card;

        [AssetsOnly]
        [ShowInInspector]
        [SerializeField]
        public GameObject[] cards;
    }
}
