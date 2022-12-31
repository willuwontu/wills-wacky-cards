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
    public abstract class CustomMechanicCard : CustomClassCard
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

        public override void OnCardBuild()
        {
            MechCardInfo card = this.GetComponent<MechCardInfo>();

            if (card != null)
            {
                card.accessory = this.GetAccessory();
            }
        }

        private void Start()
        {
            if (this.GetComponentInChildren<MechCardVisuals>())
            {
                this.GetComponentInChildren<MechCardVisuals>().modNameText.text = GetModName().Sanitize();
            }

            Callback();
        }
    }
}
