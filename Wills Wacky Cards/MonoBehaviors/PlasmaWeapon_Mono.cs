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
        private static PhotonView view;

        // Heat Bar stuff, god bless Boss Sloth
        private GameObject chargeBarObj;
        public Image chargeImage;
        public Image whiteImage;
        private float chargeTarget;
        //private HeatBar heatBar;

        private void Start()
        {
            data = GetComponentInParent<CharacterData>();
            chargeBarObj = gameObject.transform.Find("WobbleObjects/ChargeBar").gameObject;
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
                    view = this.photonView;
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
            
            chargeTarget = gun.currentCharge;
            chargeImage.fillAmount = chargeTarget;
            whiteImage.fillAmount = chargeTarget;
            chargeImage.color = new Color(Mathf.Clamp(0.5f - chargeTarget, 0f, 1f), 1f - (chargeTarget) * 0.85f, chargeTarget, 1f);
        }

        public static void ChargeGun()
        {
            view.RPC(nameof(RPCA_ChargeGun), RpcTarget.AllViaServer);
            //gun.currentCharge = Mathf.Clamp(gun.currentCharge + TimeHandler.fixedDeltaTime / gun.GetAdditionalData().chargeTime, 0f, 1f);
        }

        [PunRPC]
        private void RPCA_ChargeGun()
        {
            gun.currentCharge = Mathf.Clamp(gun.currentCharge + TimeHandler.fixedDeltaTime / gun.GetAdditionalData().chargeTime, 0f, 1f);
        }

        public static void FirePlasmaGun()
        {
            view.RPC(nameof(RPCA_FirePlasmaGun), RpcTarget.AllViaServer);
        }

        [PunRPC]
        private void RPCA_FirePlasmaGun()
        {
            UnityEngine.Debug.Log(string.Format("[WWC][Plasma Weapon] Charged shot was fired at {0:F1}% charge.", gun.currentCharge * 100f));
            chargeToUse = gun.currentCharge;
            weaponHandler.gun.Attack(gun.currentCharge, false, gun.currentCharge * gun.chargeDamageMultiplier, 1f, true);
            gun.currentCharge = 0f;
            gun.GetAdditionalData().beginCharge = false;
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            ProjectileHit bullet = obj.GetComponent<ProjectileHit>();
            MoveTransform move = obj.GetComponent<MoveTransform>();
            move.localForce *= 1 + chargeToUse * gun.chargeSpeedTo;
            //this.photonView.RPC("RPCA_ApplyChargeVel", RpcTarget.All, move, chargeToUse, gun.chargeSpeedTo);
        }

        [PunRPC]
        private void RPCA_ApplyChargeVel(MoveTransform move, float charge, float chargeSpeed, PhotonMessageInfo info)
        {
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