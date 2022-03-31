using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WWC.Extensions;
using WWC.Interfaces;

namespace WWC.MonoBehaviours
{
    class WaveMover_BulletMono : MonoBehaviour
    {
        public float amplitude = 1f;
        public float frequency = 1f;

        private void Update()
        {
            base.transform.root.position += base.transform.right * Mathf.Cos(Time.time * 10f * frequency) * 10f * this.amplitude * Time.smoothDeltaTime;
        }
    }

    class WaveMover_Mono : MonoBehaviour, IGameStartHookHandler
    {
        private CharacterData data;
        private Player player;
        private Gun gun;
        private WeaponHandler weaponHandler;

        private void Start()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            data = GetComponentInParent<CharacterData>();
            player = data.player;
            weaponHandler = data.weaponHandler;
            gun = weaponHandler.gun;
            gun.ShootPojectileAction += OnShootProjectileAction;
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            ProjectileHit bullet = obj.GetComponent<ProjectileHit>();

            var wave = bullet.gameObject.AddComponent<WaveMover_BulletMono>();

            var amp = UnityEngine.Random.Range((gun.GetAdditionalData().amplitude - 1f) / 2f, (gun.GetAdditionalData().amplitude - 1f) * 1.2f) + 1f;
            var freq = UnityEngine.Random.Range((gun.GetAdditionalData().frequency - 1f) / 2f, (gun.GetAdditionalData().frequency - 1f) * 1.2f) + 1f;

            amp = amp * (UnityEngine.Random.Range(0, 2) * 2 - 1);
            freq = freq * (UnityEngine.Random.Range(0, 2) * 2 - 1);

            wave.frequency = freq;
            wave.amplitude = amp;
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
            gun.ShootPojectileAction -= OnShootProjectileAction;
        }
    }
}
