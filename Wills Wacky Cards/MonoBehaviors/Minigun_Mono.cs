using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WillsWackyCards.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

namespace WillsWackyCards.MonoBehaviours
{
    [DisallowMultipleComponent]
    public class Minigun_Mono : MonoBehaviourPun
    {
        public float heat = 0.0f;
        public float heatCap = 3f;
        private bool overheated = false;
        public float coolPerSecond = 1.875f;
        public float secondsBeforeStartToCool = 0.1f;
        private float cooldownTimeRemaining = 0.1f;
        public float overheatBonusCooldownTime = 0.5f;
        private bool coroutineStarted;
        private float minigunDamageM = 0.035f;
        public float heatPerBullet = 0.02f;
        private Gun gun;
        private GunAmmo gunAmmo;
        private CharacterData data;
        private WeaponHandler weaponHandler;
        private Player player;

        // Heat Bar stuff, god bless Boss Sloth
        public Image overheatImage;
        private GameObject heatBarObj;
        public Image heatImage;
        public Image whiteImage;
        private float heatTarget;
        //private HeatBar heatBar;

        private void Start()
        {
            data = GetComponentInParent<CharacterData>();
            heatBarObj = gameObject.transform.Find("WobbleObjects/HeatBar").gameObject;
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
                    overheatImage = gunAmmo.reloadAnim.GetComponent<Image>();
                }
            }

            if (!(player is null) && player.gameObject.activeInHierarchy && !coroutineStarted)
            {
                coroutineStarted = true;
                StartCoroutine(Cooldown());
                InvokeRepeating(nameof(CheckIfValid), 0, 1f);
            }
            if (!(player is null))
            {
                UpdateHeatBar();
            }
        }

        private IEnumerator Cooldown()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(TimeHandler.fixedDeltaTime);
                if (gun.spread < 0.15f)
                {
                    gun.spread = 0.15f;
                }
                if (cooldownTimeRemaining > 0)
                {
                    cooldownTimeRemaining -= TimeHandler.fixedDeltaTime;
                }
                if ((cooldownTimeRemaining <= 0) && (heat > 0f))
                {
                    heat -= coolPerSecond * TimeHandler.fixedDeltaTime;
                }
                if (overheated && (heat <= 0f))
                {
                    overheated = false;
                    gun.GetAdditionalData().overHeated = false;
                    gunAmmo.ReloadAmmo(true);
                } 
            }
        }

        private void UpdateHeatBar()
        {
            heatTarget = heat / heatCap;
            heatImage.fillAmount = heatTarget;
            whiteImage.fillAmount = heatTarget;
            if (!overheated)
            {
                heatImage.color = new Color(heatTarget, 1f - (heatTarget) * 0.85f, Mathf.Clamp(0.5f - heatTarget, 0f, 1f), 1f);
                gunAmmo.ReloadAmmo();
            }
            if (overheated)
            {
                overheatImage.fillAmount = heatTarget;
            }
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            ProjectileHit bullet = obj.GetComponent<ProjectileHit>();
            //UnityEngine.Debug.Log(string.Format("[WWC][Minigun] {0} heat", heat));
            bullet.damage *= minigunDamageM;
            heat += heatPerBullet;
            cooldownTimeRemaining = secondsBeforeStartToCool;
            if (heat < heatCap)
            {
                gunAmmo.ReDrawTotalBullets(); 
            }
            else
            {
                overheated = true;
                gun.GetAdditionalData().overHeated = true;
                cooldownTimeRemaining += overheatBonusCooldownTime;
                heatImage.color = Color.red;
            }
        }

        private void CheckIfValid()
        {
            var haveMinigun = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName == "Minigun")
                {
                    haveMinigun = true;
                    break;
                }
            }

            if (!haveMinigun)
            {
                Destroy(heatBarObj);
                gun.ShootPojectileAction -= OnShootProjectileAction;
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            gun.ShootPojectileAction -= OnShootProjectileAction;
            Destroy(heatBarObj);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}