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
    public class CardReq
    {
        [AssetsOnly]
        [ShowInInspector]
        [SerializeField]
        public GameObject[] cards;
    }
}
