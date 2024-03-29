﻿using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WWC.Extensions;
using WWC.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sonigon;
using Photon.Pun;
using BepInEx.Bootstrap;
using System.Linq;

namespace WWC.MonoBehaviours
{
    [DisallowMultipleComponent]
    public class PlasmaWeapon_Mono : MonoBehaviour
    {
        public float chargeToUse = 0f;
        public bool canShoot = true;
        public float chargeTime => GunChargePatch.Extensions.GunExtensions.GetAdditionalData(this.gun).chargeTime;
        public float MaxCharge => GunChargePatch.Extensions.GunExtensions.GetAdditionalData(this.gun).maxCharge;

        private Gun gun;
        private GunAmmo gunAmmo;
        private CharacterData data;
        private WeaponHandler weaponHandler;
        private Player player;
        private GeneralInput input;
        private PhotonView view;

        // Heat Bar stuff, god bless Boss Sloth
        private GameObject chargeBarObj;
        public Image chargeImage;
        private float chargeTarget;
        private Coroutine syncRoutine;
        //private HeatBar heatBar;

        private void Start()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            data = GetComponentInParent<CharacterData>();
            player = data.player;

            { // Create UI
                chargeBarObj = Instantiate(player.transform.Find("WobbleObjects/Healthbar"), player.transform.Find("WobbleObjects")).gameObject;
                chargeBarObj.name = "ChargeBar";
                UnityEngine.GameObject.DestroyImmediate(chargeBarObj.GetComponentInChildren<HealthBar>());
                chargeBarObj.transform.Translate(new Vector3(.95f, -1.1f, 0));
                chargeBarObj.transform.localScale.Set(0.5f, 1f, 1f);
                chargeBarObj.transform.localScale = new Vector3(0.6f, 1.4f, 1f);
                chargeBarObj.transform.Rotate(0f, 0f, 90f);
                var nameLabel = chargeBarObj.transform.Find("Canvas/PlayerName").gameObject;
                var crown = chargeBarObj.transform.Find("Canvas/CrownPos").gameObject;

                var grid = chargeBarObj.transform.Find("Canvas/Image/Grid");
                grid.gameObject.SetActive(true);
                grid.localScale = new Vector3(1f, .4f, 1f);

                var gridBox = Instantiate(grid.transform.Find("Grid (8)"), grid);
                gridBox.name = "Grid (9)";

                for (int i = 1; i <= 9; i++)
                {
                    gridBox = grid.transform.Find($"Grid ({i})");
                    gridBox.localScale = new Vector3(2f, 1f, 1f);
                    if (i > 4)
                    {
                        gridBox.gameObject.SetActive(false);
                    }
                }

                this.chargeImage = chargeBarObj.transform.Find("Canvas/Image/Health").GetComponent<Image>();
                UnityEngine.GameObject.Destroy(chargeBarObj.transform.Find("Canvas/Image/White").GetComponent<Image>().gameObject);
                this.chargeImage.name = "Charge";
                this.chargeImage.color = new Color(255, 255, 255);
                this.chargeImage.SetAlpha(1);
                Destroy(nameLabel);
                Destroy(crown);

                if (Chainloader.PluginInfos.Keys.Contains("com.bosssloth.rounds.LocalZoom"))
                {
                    LocalZoom.LocalZoom.MakeObjectHidden(this.chargeBarObj.transform);
                    LocalZoom.Extensions.CharacterDataExtension.GetData(this.data).allWobbleImages.AddRange(chargeBarObj.GetComponentsInChildren<Image>());
                }
            }

            player.GetComponentInChildren<ChildRPC>().childRPCsInt.Add("ChargeSync", RPCA_SyncCurrent);

            player = data.player;
            weaponHandler = data.weaponHandler;
            gun = weaponHandler.gun;
            gunAmmo = gun.GetComponentInChildren<GunAmmo>();
            input = data.input;
            view = player.data.view;
        }

        private void Update()
        {
            if (!(player is null) && player.gameObject.activeInHierarchy && (!(syncRoutine != null)))
            {
                syncRoutine = WillsWackyCards.instance.StartCoroutine(PeriodicSync());
            }

            //AttackHandler();
            UpdateChargeBar();
        }

        private IEnumerator PeriodicSync()
        {
            while (true)
            {
                if (this.view.IsMine && player.gameObject.activeInHierarchy)
                {
                    player.GetComponentInChildren<ChildRPC>().CallFunction("ChargeSync", (int)(gun.currentCharge * 100f));
                }
                yield return new WaitForSecondsRealtime(0.15f);
            }
        }

        private void RPCA_SyncCurrent(int charge)
        {
            gun.currentCharge = (charge / 100f);
        }

        [PunRPC]
        private void RPCA_SyncCharge(float charge)
        {
            gun.currentCharge = charge;
        }

        private void AttackHandler()
        {
            if (input.shootIsPressed && !player.data.dead && (bool)(typeof(PlayerVelocity).GetField("simulated", BindingFlags.Instance | BindingFlags.GetField |
                        BindingFlags.NonPublic).GetValue(player.data.playerVel)) && (0 < (int)typeof(GunAmmo).GetField("currentAmmo", BindingFlags.Instance | BindingFlags.GetField |
                        BindingFlags.NonPublic).GetValue(gunAmmo)))
            {
                if (gun.currentCharge < this.MaxCharge)
                {
                    gun.currentCharge = Mathf.Clamp(gun.currentCharge + (TimeHandler.deltaTime / this.chargeTime * this.MaxCharge), 0f, this.MaxCharge);
                    //UnityEngine.Debug.Log(string.Format("[WWC][Plasma Weapon] Gun is currently {0:F1}% charged.", gun.currentCharge * 100f)); 
                }
            }
        }

        private void UpdateChargeBar()
        {
            chargeTarget = gun.currentCharge/this.MaxCharge;
            chargeImage.fillAmount = chargeTarget;
            chargeImage.color = new Color(Mathf.Clamp(0.5f - chargeTarget, 0f, 1f), 1f - (chargeTarget) * 0.85f, chargeTarget, 1f);
        }

        private void OnDestroy()
        {
            try
            {
                InterfaceGameModeHooksManager.instance.RemoveHooks(this);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Error thrown when removing hooks.");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                gun.useCharge = false;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Error thrown when setting use charge to false.");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                player.GetComponentInChildren<ChildRPC>().childRPCsInt.Remove("ChargeSync");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Error thrown when removing charge sync rpc.");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                if (Chainloader.PluginInfos.Keys.Contains("com.bosssloth.rounds.LocalZoom"))
                {
                    foreach (var image in chargeBarObj.GetComponentsInChildren<Image>())
                    {
                        LocalZoom.Extensions.CharacterDataExtension.GetData(this.data).allWobbleImages.RemoveAll(img => img == image);
                    }
                } 
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Error thrown when removing images from local zoom images.");
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                UnityEngine.GameObject.Destroy(chargeBarObj);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Error thrown when destroying charge bar obj.");
                UnityEngine.Debug.LogException(e);
            }
}

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}