using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WillsWackyCards.Extensions;
using System;
using System.Collections.Generic;
using Sonigon;
using Photon.Pun;

namespace WillsWackyCards.MonoBehaviours
{
    [DisallowMultipleComponent]
    public class PlasmaWeapon_Mono : MonoBehaviourPun
    {
        public float chargeToUse = 0f;

        private bool coroutineStarted;
        private Gun gun;
        private GunAmmo gunAmmo;
        private CharacterData data;
        private WeaponHandler weaponHandler;
        private Player player;

        // Heat Bar stuff, god bless Boss Sloth
        private GameObject chargeBarObj;
        public Image chargeImage;
        public Image whiteImage;
        //private float chargeTarget;
        //private HeatBar heatBar;

        private void Start()
        {
            data = GetComponentInParent<CharacterData>();
            chargeBarObj = gameObject.transform.Find("WobbleObjects/ChargeBar").gameObject;
            chargeBarObj.transform.Find("Canvas/Image/Health").GetComponent<Image>().color = new Color(255,255,255);
            chargeBarObj.transform.Find("Canvas/Image/Health").GetComponent<Image>().SetAlpha(1);
            chargeImage = chargeBarObj.transform.Find("Canvas/Image/Health").GetComponent<Image>();
            whiteImage = chargeBarObj.transform.Find("Canvas/Image/White").GetComponent<Image>();
            whiteImage.SetAlpha(0);
        }

        private void Update()
        {
            if (!player)
            {
                if (!(data is null))
                {
                    player = data.player;
                    weaponHandler = data.weaponHandler;
                    gun = weaponHandler.gun;
                    gunAmmo = gun.GetComponentInChildren<GunAmmo>();
                    gun.ShootPojectileAction += OnShootProjectileAction;
                }
            }

            if (!(player is null) && player.gameObject.activeInHierarchy && !coroutineStarted)
            {
                coroutineStarted = true;
                InvokeRepeating(nameof(CheckIfValid), 0, 1f);
            }

            UpdateChargeBar();
        }

        private void UpdateChargeBar()
        {
            this.photonView.RPC("RPCA_UpdateChargeBar", RpcTarget.All, new object[] { gun.currentCharge, chargeImage, whiteImage });
        }

        [PunRPC]
        private void RPCA_UpdateChargeBar(float charge, Image chargeImage, Image whiteImage)
        {
            var chargeTarget = charge;
            chargeImage.fillAmount = chargeTarget;
            whiteImage.fillAmount = chargeTarget;
            chargeImage.color = new Color(Mathf.Clamp(0.5f - chargeTarget, 0f, 1f), 1f - (chargeTarget) * 0.85f, chargeTarget, 1f);
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            this.photonView.RPC("RPCA_ApplyChargeVel", RpcTarget.All, new object[] { obj, chargeToUse, gun.chargeSpeedTo });
        }

        [PunRPC]
        private void RPCA_ApplyChargeVel(GameObject obj, float charge, float chargeSpeed, PhotonMessageInfo info)
        {
            ProjectileHit bullet = obj.GetComponent<ProjectileHit>();
            MoveTransform move = obj.GetComponent<MoveTransform>();
            move.localForce *= 1 + charge * chargeSpeed;
            UnityEngine.Debug.Log($"[WWC][PlasmaMono][Photon] {info.Sender} {info.photonView} {info.SentServerTime} Increasing bullet velocity by {string.Format("{0:F0}", charge * chargeSpeed * 100)}%.");
        }

        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName == "Plasma Rifle" || player.data.currentCards[i].cardName == "Plasma Shotgun")
                {
                    haveCard = true;
                    break;
                }
            }

            if (!haveCard)
            {
                Destroy(chargeBarObj);
                gun.ShootPojectileAction -= OnShootProjectileAction;
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            gun.ShootPojectileAction -= OnShootProjectileAction;
            Destroy(chargeBarObj);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}