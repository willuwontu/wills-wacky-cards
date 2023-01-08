using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.Cards;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using ClassesManagerReborn;
using WWC.Cards;
using WillsWackyManagers.UnityTools;

namespace WWC.UnityTools
{
    [Serializable]
    public class ClassCardBuilder : CardBuilder
    {
        [SerializeField]
        public string className;

        [AssetsOnly]
        [ShowInInspector]
        [SerializeField]
        [NonSerialized]
        [OdinSerialize]
        [Searchable]
        public ClassCardRegistrationInfo[] cardsToRegister = new ClassCardRegistrationInfo[0];

        [AssetsOnly]
        [ShowInInspector]
        [NonSerialized]
        [OdinSerialize]
        [Searchable]
        public CardCardList[] blackList = new CardCardList[0];

        [AssetsOnly]
        [ShowInInspector]
        [NonSerialized]
        [OdinSerialize]
        [Searchable]
        public CardCardList[] whiteList = new CardCardList[0];

        public override void BuildCards()
        {
            for (int i = 0; i < cardsToRegister.Length; i++)
            {
                var customCard = cardsToRegister[i].card.GetComponent<CustomClassCard>();

                if (customCard)
                {
                    customCard.BuildUnityClassCard(StandardCallback);

                    CardInfo card = customCard.GetComponent<CardInfo>();

                    switch (cardsToRegister[i].requirementType)
                    {
                        case RequirementsType.None:
                            ClassesRegistry.Register(card, cardsToRegister[i].cardType, cardsToRegister[i].maxAllowed);
                            break;
                        case RequirementsType.Single:
                            ClassesRegistry.Register(card, cardsToRegister[i].cardType, cardsToRegister[i].singleReq.GetComponent<CardInfo>(), cardsToRegister[i].maxAllowed);
                            break;
                        case RequirementsType.List:
                            ClassesRegistry.Register(card, cardsToRegister[i].cardType, cardsToRegister[i].listReq.Select(c => c.GetComponent<CardInfo>()).ToArray(), cardsToRegister[i].maxAllowed);
                            break;
                        case RequirementsType.MultiList:
                            ClassesRegistry.Register(card, cardsToRegister[i].cardType, cardsToRegister[i].multilistReq.Select(con => con.cards.Select(c => c.GetComponent<CardInfo>()).ToArray()).ToArray(), cardsToRegister[i].maxAllowed);
                            break;
                    }
                }
            }

            for (int i = 0; i < blackList.Length; i++)
            {
                CardInfo card = blackList[i].card.GetComponent<CardInfo>();
                for (int j = 0; j < blackList[i].cards.Length; j++)
                {
                    CardInfo otherCard = blackList[i].cards[j].GetComponent<CardInfo>();

                    if (card && otherCard)
                    {
                        ClassesRegistry.Get(card).Blacklist(otherCard);
                    }
                }
            }

            for (int i = 0; i < whiteList.Length; i++)
            {
                CardInfo card = whiteList[i].card.GetComponent<CardInfo>();
                for (int j = 0; j < whiteList[i].cards.Length; j++)
                {
                    CardInfo otherCard = whiteList[i].cards[j].GetComponent<CardInfo>();

                    if (card && otherCard)
                    {
                        ClassesRegistry.Get(card).Whitelist(otherCard);
                    }
                }
            }
        }
    }
}
