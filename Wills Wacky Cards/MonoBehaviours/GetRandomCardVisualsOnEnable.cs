using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnboundLib;
using UnboundLib.Utils;
using UnityEngine;

namespace WWC.MonoBehaviours
{
    public class GetRandomCardVisualsOnEnable : MonoBehaviour
    {
        GameObject cardObj = null;
        private void Awake()
        {
            if (!this.gameObject.GetComponent<RectTransform>())
            {
                UnityEngine.GameObject.DestroyImmediate(this);
            }
        }

        private CardInfo GetRandomCardWithArt()
        {
            CardInfo[] cards = CardManager.cards.Values.Where(card => card.cardInfo.cardArt != null && card.cardInfo.cardName != "Portable Fabricator").Select(card => card.cardInfo).ToArray();

            return cards[UnityEngine.Random.Range(0, cards.Length)];
        }

        private void OnEnable()
        {
            if (cardObj != null)
            {
                UnityEngine.GameObject.Destroy(cardObj);
            }

            var card = GetRandomCardWithArt();

            RectTransform rect;

            try
            {
                cardObj = Instantiate<GameObject>(card.gameObject.transform.GetChild(0).GetChild(0).gameObject, this.gameObject.transform);
            }
            catch (Exception)
            {
                cardObj = Instantiate<GameObject>(card.gameObject, this.gameObject.transform);
                var temp = cardObj;
                cardObj = cardObj.GetComponentInChildren<Canvas>().gameObject;
                cardObj.transform.SetParent(this.gameObject.transform);

                UnityEngine.GameObject.Destroy(temp);
            }
            cardObj.SetActive(true);

            rect = cardObj.GetOrAddComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            var rarityThings = cardObj.GetComponentsInChildren<CardRarityColor>();

            foreach (var thing in rarityThings)
            {
                try
                {
                    thing.GetComponentInParent<CardVisuals>().toggleSelectionAction = (Action<bool>)Delegate.Remove(thing.GetComponentInParent<CardVisuals>().toggleSelectionAction, new Action<bool>(thing.Toggle));
                    UnityEngine.GameObject.Destroy(thing);
                }
                catch (Exception)
                {
                    UnityEngine.GameObject.Destroy(thing);
                }
            }

            var canvasGroups = cardObj.GetComponentsInChildren<CanvasGroup>();
            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 1;
            }

            UnityEngine.GameObject.Destroy(cardObj.transform.Find("Back").gameObject);

            var artHolder = cardObj.transform.Find("Front/Background/Art").gameObject;

            var art = Instantiate<GameObject>(card.cardArt, artHolder.transform);
            rect = art.GetOrAddComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
        }

        private void OnDisable()
        {
            UnityEngine.GameObject.Destroy(cardObj);
            cardObj = null;
        }
    }
}

