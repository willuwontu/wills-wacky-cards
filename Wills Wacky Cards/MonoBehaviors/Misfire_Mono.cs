using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WillsWackyCards.Extensions;
using System;

namespace WillsWackyCards.MonoBehaviours
{
    public class Misfire_Mono : MonoBehaviour
    {
        public int misfireChance = 0;
        private static System.Random random = new System.Random();
        private bool coroutineStarted;
        private Gun gun;
        private GunAmmo gunAmmo;
        private CharacterData data;
        private WeaponHandler weaponHandler;
        private Player player;

        private void Start()
        {
            data = GetComponentInParent<CharacterData>();
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
        }


        private void OnShootProjectileAction(GameObject obj)
        {
            var roll = random.Next(100);
            if (roll < misfireChance)
            {
                gunAmmo.SetFieldValue("currentAmmo", 0);
            }
        }
        private void CheckIfValid()
        {
            var haveMisfire = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName == "Misfire")
                {
                    haveMisfire = true;
                    break;
                }
            }

            if (!haveMisfire)
            {
                gun.ShootPojectileAction -= OnShootProjectileAction;
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            gun.ShootPojectileAction -= OnShootProjectileAction;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}