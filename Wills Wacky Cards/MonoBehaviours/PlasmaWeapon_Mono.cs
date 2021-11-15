using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WWC.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using Sonigon;
using Photon.Pun;

namespace WWC.MonoBehaviours
{
    [DisallowMultipleComponent]
    public class PlasmaWeapon_Mono : MonoBehaviourPun
    {
        public float chargeToUse = 0f;
        public bool canShoot = true;

        private bool coroutineStarted;
        private Gun gun;
        private GunAmmo gunAmmo;
        private CharacterData data;
        private WeaponHandler weaponHandler;
        private Player player;
        private GeneralInput input;
        private GeneralInput inputSync = new GeneralInput();
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
                    input = data.input;
                    view = this.photonView;
                }
            }

            if (!(player is null) && player.gameObject.activeInHierarchy && !coroutineStarted)
            {
                coroutineStarted = true;
                InvokeRepeating(nameof(CheckIfValid), 0, 1f);
                StartCoroutine(PeriodicSync());
            }

            SyncAttackInputs();
            AttackHandler();
            UpdateChargeBar();
        }

        private void SyncAttackInputs()
        {
            //if (this.photonView.IsMine)
            //{
            //    if (input.shootWasPressed != inputSync.shootWasPressed || input.shootWasReleased != inputSync.shootWasReleased)
            //    {
            //        this.photonView.RPC(nameof(RPCA_SyncAttackInputs), RpcTarget.All, input.shootIsPressed, input.shootWasPressed, input.shootWasReleased);
            //    }
            //}
            inputSync.shootIsPressed = input.shootIsPressed;
            inputSync.shootWasPressed = input.shootWasPressed;
            inputSync.shootWasReleased = input.shootWasReleased;
        }

        [PunRPC]
        private void RPCA_SyncAttackInputs(bool isPressed, bool wasPressed, bool wasReleased)
        {
            inputSync.shootIsPressed = isPressed;
            inputSync.shootWasPressed = wasPressed;
            inputSync.shootWasReleased = wasReleased;
        }

        private IEnumerator PeriodicSync()
        {
            while (true)
            {
                if (this.photonView.IsMine)
                {
                    this.photonView.RPC(nameof(RPCA_SyncCharge), RpcTarget.All, gun.currentCharge);
                }
                yield return new WaitForSecondsRealtime(TimeHandler.deltaTime * 5f);
            }
        }

        [PunRPC]
        private void RPCA_SyncCharge(float charge)
        {
            gun.currentCharge = charge;
        }

        private void AttackHandler()
        {
            var ready = true;
            if (!gun)
            {
                ready = false;
            }
            if (!gun.IsReady(0f))
            {
                ready = false;
            }

            if (ready)
            {
                if (inputSync.shootWasPressed && (int)gunAmmo.GetFieldValue("currentAmmo") > 0 && canShoot)
                {
                    gun.GetAdditionalData().beginCharge = true;
                    UnityEngine.Debug.Log("[WWC][Plasma Weapon] Beginning to charge shot.");
                }
                else if (inputSync.shootWasPressed)
                {
                    SoundManager.Instance.Play(weaponHandler.soundCharacterCantShoot, weaponHandler.transform);
                }

                if (inputSync.shootIsPressed)
                {
                    if (gun.GetAdditionalData().beginCharge && gun.currentCharge < 1f)
                    {
                        gun.currentCharge = Mathf.Clamp(gun.currentCharge + TimeHandler.fixedDeltaTime / gun.GetAdditionalData().chargeTime, 0f, 1f);
                        //UnityEngine.Debug.Log(string.Format("[WWC][Plasma Weapon] Gun is currently {0:F1}% charged.", gun.currentCharge * 100f)); 
                    }
                }
                if (inputSync.shootWasReleased && canShoot)
                {
                    if (gun.GetAdditionalData().beginCharge)
                    {
                        UnityEngine.Debug.Log(string.Format("[WWC][Plasma Weapon] Charged shot was fired at {0:F1}% charge.", gun.currentCharge * 100f));
                        chargeToUse = gun.currentCharge;
                        weaponHandler.gun.Attack(gun.currentCharge, false, gun.currentCharge * gun.chargeDamageMultiplier, 1f, true);
                        gun.currentCharge = 0f;
                        gun.GetAdditionalData().beginCharge = false;
                    }
                } 
            }
        }

        private void UpdateChargeBar()
        {
            chargeTarget = gun.currentCharge;
            chargeImage.fillAmount = chargeTarget;
            whiteImage.fillAmount = chargeTarget;
            chargeImage.color = new Color(Mathf.Clamp(0.5f - chargeTarget, 0f, 1f), 1f - (chargeTarget) * 0.85f, chargeTarget, 1f);
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            //ProjectileHit bullet = obj.GetComponent<ProjectileHit>();
            //MoveTransform move = obj.GetComponent<MoveTransform>();
            var velBoost = obj.AddComponent<VelocityBooster>();
            if (this.photonView.IsMine)
            {
                StartCoroutine(velBoost.ChangeVelocity(1 + chargeToUse * gun.chargeSpeedTo));
            }
        }

        private class VelocityBooster : MonoBehaviourPun
        {
            private bool updated = false;
            public IEnumerator ChangeVelocity(float speedMult)
            {
                yield return 0;

                this.photonView.RPC(nameof(RPCA_UpdateSpeed), RpcTarget.All, speedMult);

                yield return 0;

                this.photonView.RPC(nameof(RPCA_UpdateSpeed), RpcTarget.All, speedMult);

                yield return 0;

                this.photonView.RPC(nameof(RPCA_UpdateSpeed), RpcTarget.All, speedMult);

                yield return 0;

                this.photonView.RPC(nameof(RPCA_UpdateSpeed), RpcTarget.All, speedMult);

                yield return 0;

                this.photonView.RPC(nameof(RPCA_UpdateSpeed), RpcTarget.All, speedMult);
            }

            [PunRPC]
            private void RPCA_UpdateSpeed(float speedMult)
            {
                if (!updated)
                {
                    updated = true;
                    var move = base.GetComponentInParent<MoveTransform>();
                    move.velocity *= speedMult;
                    //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][PlasmaMono] Increasing bullet velocity by {string.Format("{0:F0}", (speedMult - 1f) * 100)}%.");
                    this.ExecuteAfterFrames(5, delegate(){ Destroy(this); });

                }
            }
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