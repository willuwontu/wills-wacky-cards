using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WWC.Extensions;
using System;

namespace WWC.MonoBehaviours
{
    public class Gatling_Mono : MonoBehaviour
    {
        private int stacks = 0;
        private int maxStacks = 20;
        public float rampUp = 1f;
        private float timeSinceShot = 1f;
        private bool coroutineStarted;
        private Gun gun;
        private GunAmmo gunAmmo;
        private CharacterData data;
        private WeaponHandler weaponHandler;
        private CharacterStatModifiers stats;
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
                    stats = data.stats;
                    weaponHandler = data.weaponHandler;
                    gun = weaponHandler.gun;
                    gunAmmo = gun.GetComponentInChildren<GunAmmo>();
                    gun.ShootPojectileAction += OnShootProjectileAction;
                    stats.OutOfAmmpAction += OnReloadAction;
                }
            }

            if (!(player is null) && player.gameObject.activeInHierarchy && !coroutineStarted)
            {
                coroutineStarted = true;
                InvokeRepeating(nameof(SpinDown), 0, TimeHandler.deltaTime);
                InvokeRepeating(nameof(CheckIfValid), 0, 1f);
            }
        }

        private void SpinDown()
        {
            if (timeSinceShot > 0f)
            {
                timeSinceShot -= TimeHandler.deltaTime;
            }
            else
            {
                if (stacks > 0)
                {
                    gun.attackSpeed /= rampUp;
                    stacks -= 1;
                    timeSinceShot += 0.1f;
                    //UnityEngine.Debug.Log(string.Format("Gatling stack Lost, {0} stacks remaining.", stacks));
                    //UnityEngine.Debug.Log(string.Format("Attack Speed is now {0}", gun.attackSpeedMultiplier));
                }
                
            }
        }

        private void OnReloadAction(int maxAmmo)
        {
            var ratio = ((gunAmmo.reloadTime + gunAmmo.reloadTimeAdd) * gunAmmo.reloadTimeMultiplier)/1.5;
            //UnityEngine.Debug.Log($"A reload time of {(gunAmmo.reloadTime + gunAmmo.reloadTimeAdd) * gunAmmo.reloadTimeMultiplier} and {ratio * 100}% reload to stack time ratio.");
            var loss = Mathf.Clamp((int)(ratio * stacks), 0, stacks);
            //UnityEngine.Debug.Log(string.Format("{0} gatling stacks lost while reloading", loss));
            while (loss > 0)
            {
                gun.attackSpeed /= rampUp;
                stacks -= 1;
                loss -= 1;
                //UnityEngine.Debug.Log(string.Format("Attack Speed is now {0}", gun.attackSpeed));
            }
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            if (stacks < maxStacks)
            {
                gun.attackSpeed *= rampUp;
                stacks += 1;
                timeSinceShot = 1f;
                //UnityEngine.Debug.Log(string.Format("Gatling stack added, {0} stacks total.", stacks));
                //UnityEngine.Debug.Log(string.Format("Attack Speed is now {0}", gun.attackSpeed)); 
            }
        }
        private void CheckIfValid()
        {
            var haveGatling = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName == "Gatling Gun")
                {
                    haveGatling = true;
                    break;
                }
            }

            if (!haveGatling)
            {
                stats.OutOfAmmpAction -= OnReloadAction;
                gun.ShootPojectileAction -= OnShootProjectileAction;
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            stats.OutOfAmmpAction -= OnReloadAction;
            gun.ShootPojectileAction -= OnShootProjectileAction;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}