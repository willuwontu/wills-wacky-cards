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

namespace WWC.Cards
{
    public abstract class CustomMechanicCard : CustomCard
    {
        //public void BuildUnityCard()
        //{
        //    MethodInfo buildAction = typeof(CustomMechanicCard).GetMethod(nameof(CustomMechanicCard.BuildUnityCard));
        //    MethodInfo generic = buildAction.MakeGenericMethod(this.GetType());

        //    generic.Invoke(null, new object[] { this.gameObject, null });
        //}


        protected virtual GameObject GetAccessory()
        {
            return WillsWackyManagers.WillsWackyManagers.instance.WWMAssets.LoadAsset<GameObject>("Wrench Accessory");
        }

        public static void BuildUnityMechanicCard<T>(GameObject cardPrefab, Action<CardInfo> callback) where T : CustomMechanicCard
        {
            BuildUnityCard<T>(cardPrefab, callback);

            MechCardInfo card = cardPrefab.GetComponent<MechCardInfo>();
            CustomMechanicCard customCard = cardPrefab.GetOrAddComponent<T>();

            if (card != null)
            {
                card.accessory = customCard.GetAccessory();
            }
        }

        private void Awake()
        {
            cardInfo = GetComponent<CardInfo>();
            gun = GetComponent<Gun>();
            cardStats = GetComponent<ApplyCardStats>();
            statModifiers = GetComponent<CharacterStatModifiers>();
            block = gameObject.GetOrAddComponent<Block>();
            SetupCard(cardInfo, gun, cardStats, statModifiers, block);
        }

        private void Start()
        {
            if (this.GetComponentInChildren<MechCardVisuals>())
            {
                this.GetComponentInChildren<MechCardVisuals>().modNameText.text = GetModName().Sanitize();
            }

            Callback();
        }

        public static void BuildUnityCard<T>(GameObject cardPrefab, Action<CardInfo> callback) where T : CustomMechanicCard
        {
            CardInfo cardInfo = cardPrefab.GetComponent<CardInfo>();
            CustomMechanicCard customCard = cardPrefab.GetOrAddComponent<T>();

            cardInfo.cardBase = customCard.GetCardBase();
            cardInfo.cardStats = customCard.GetStats();
            cardInfo.cardName = customCard.GetTitle();
            cardInfo.gameObject.name = $"__{customCard.GetModName()}__{customCard.GetTitle()}".Sanitize();
            cardInfo.cardDestription = customCard.GetDescription();
            cardInfo.sourceCard = cardInfo;
            cardInfo.rarity = customCard.GetRarity();
            cardInfo.colorTheme = customCard.GetTheme();
            cardInfo.cardArt = customCard.GetCardArt();

            PhotonNetwork.PrefabPool.RegisterPrefab(cardInfo.gameObject.name, cardPrefab);

            if (customCard.GetEnabled())
            {
                CardManager.cards.Add(cardInfo.gameObject.name, new Card(customCard.GetModName().Sanitize(), Unbound.config.Bind("Cards: " + customCard.GetModName().Sanitize(), cardInfo.gameObject.name, true), cardInfo));
            }

            Unbound.Instance.ExecuteAfterFrames(5, () =>
            {
                callback?.Invoke(cardInfo);
            });
        }

        public static void RegisterUnityCard<T>(GameObject cardPrefab, Action<CardInfo> callback) where T : CustomMechanicCard
        {
            CardInfo cardInfo = cardPrefab.GetComponent<CardInfo>();
            CustomMechanicCard customCard = cardPrefab.GetOrAddComponent<T>();

            cardInfo.gameObject.name = $"__{customCard.GetModName()}__{customCard.GetTitle()}".Sanitize();

            PhotonNetwork.PrefabPool.RegisterPrefab(cardInfo.gameObject.name, cardPrefab);

            if (customCard.GetEnabled())
            {
                CardManager.cards.Add(cardInfo.gameObject.name, new Card(customCard.GetModName().Sanitize(), Unbound.config.Bind("Cards: " + customCard.GetModName().Sanitize(), cardInfo.gameObject.name, true), cardInfo));
            }

            cardInfo.ExecuteAfterFrames(5, () =>
            {
                callback?.Invoke(cardInfo);
            });
        }

        public static void RegisterUnityCard(GameObject cardPrefab, string modInitials, string cardname, bool enabled, Action<CardInfo> callback)
        {
            CardInfo cardInfo = cardPrefab.GetComponent<CardInfo>();

            cardInfo.gameObject.name = $"__{modInitials}__{cardname}".Sanitize();

            PhotonNetwork.PrefabPool.RegisterPrefab(cardInfo.gameObject.name, cardPrefab);

            if (enabled)
            {
                CardManager.cards.Add(cardInfo.gameObject.name, new Card(cardname.Sanitize(), Unbound.config.Bind("Cards: " + cardname.Sanitize(), cardInfo.gameObject.name, true), cardInfo));
            }

            cardInfo.ExecuteAfterFrames(5, () =>
            {
                callback?.Invoke(cardInfo);
            });
        }
    }
}
