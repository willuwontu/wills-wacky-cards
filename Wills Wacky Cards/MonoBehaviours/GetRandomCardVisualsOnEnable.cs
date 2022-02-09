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

            cardObj = Instantiate<GameObject>(card.gameObject, this.gameObject.transform);
            cardObj.GetComponentInChildren<CardVisuals>().firstValueToSet = true;

            var rect = cardObj.GetOrAddComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            var temp = cardObj.GetComponentInChildren<Canvas>().gameObject;
            rect = temp.GetOrAddComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            temp = temp.transform.parent.gameObject;
            rect = temp.GetOrAddComponent<RectTransform>();
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
