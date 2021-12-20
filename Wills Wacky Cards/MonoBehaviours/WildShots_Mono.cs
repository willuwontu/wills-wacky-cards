using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WWC.Extensions;
using System;

namespace WWC.MonoBehaviours
{
    public class WildShotsBullet_Mono : UnityEngine.MonoBehaviour
    {
        private void Start()
        {
            if (gameObject.transform.parent == null)
            {
                return;
            }
            var move = base.GetComponentInParent<MoveTransform>();
            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}] Bullet Velocity is ({move.velocity.x}, {move.velocity.y}, {move.velocity.z}) with a magnitude of {move.velocity.magnitude}.");
            move.velocity *= -1;
            Destroy(this);
        }
    }

public class Test
{
    private Gun gun;
}

    public class WildShots_Mono : MonoBehaviour
    {
        private GameObject bulletMono = new GameObject("Backwards", typeof(WildShotsBullet_Mono));
        public int backwardsChance = 0;
        private static System.Random random = new System.Random();
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
            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}] Player Velocity is ({((Vector2)data.playerVel.GetFieldValue("velocity")).x}, {((Vector2)data.playerVel.GetFieldValue("velocity")).y}) with a magnitude of {((Vector2)data.playerVel.GetFieldValue("velocity")).magnitude}.");
            var roll = random.Next(100);
            if (roll < backwardsChance)
            {
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Hex] Player {player.teamID} Backwards Curse activated with a roll of {roll} and a chance of {backwardsChance}%.");
                var component = obj.GetComponent<ProjectileHit>();
                var gameObject = UnityEngine.Object.Instantiate<GameObject>(bulletMono, component.transform.position, component.transform.rotation, component.transform);
            }
        }
        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName.ToLower() == "Wild Shots".ToLower())
                {
                    haveCard = true;
                    break;
                }
            }

            if (!haveCard)
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