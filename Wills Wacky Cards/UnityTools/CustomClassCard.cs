using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.Utils;
using Photon.Pun;
using WillsWackyManagers.MonoBehaviours;
using WillsWackyManagers.UnityTools;

namespace WWC.Cards
{
    public abstract class CustomClassCard : CustomCard, ISaveableCard
    {
        //public void BuildUnityCard()
        //{
        //    MethodInfo buildAction = typeof(CustomMechanicCard).GetMethod(nameof(CustomMechanicCard.BuildUnityCard));
        //    MethodInfo generic = buildAction.MakeGenericMethod(this.GetType());

        //    generic.Invoke(null, new object[] { this.gameObject, null });
        //}

        public abstract CardInfo Card { get; set; }

        public virtual void OnCardBuild()
        {

        }

        public static void BuildUnityClassCard<T>(GameObject cardPrefab, Action<CardInfo> callback) where T : CustomClassCard
        {
            BuildUnityCard<T>(cardPrefab, callback);

            CardInfo card = cardPrefab.GetComponent<CardInfo>();
            CustomClassCard customClassCard = cardPrefab.GetOrAddComponent<T>();

            customClassCard.Card = card;

            customClassCard.OnCardBuild();
        }

        public void BuildUnityClassCard(Action<CardInfo> callback)
        {
            this.BuildUnityCard(callback);

            CardInfo card = this.GetComponent<CardInfo>();
            CustomClassCard customClassCard = this;

            customClassCard.Card = card;

            this.OnCardBuild();
        }
    }
}
