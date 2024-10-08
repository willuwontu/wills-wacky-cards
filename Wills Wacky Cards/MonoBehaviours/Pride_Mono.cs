using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WWC.MonoBehaviours
{
    internal class Pride_Mono : MonoBehaviour
    {
        private CharacterData data;
        private Block block;
        private Gun gun;

        private void Start()
        {
            this.data = this.GetComponentInParent<CharacterData>();
            this.block = data.block;
            this.gun = data.weaponHandler.gun;

            block.BlockAction += PunchSelf;
            gun.ShootPojectileAction += PunchSelfGun;
        }

        private void OnDestroy()
        {
            block.BlockAction -= PunchSelf;
            gun.ShootPojectileAction -= PunchSelfGun;
        }

        private void PunchSelf(BlockTrigger.BlockTriggerType blockTriggerType)
        {
            this.data.healthHandler.TakeDamage(Vector2.up * (this.data.maxHealth * .20f), Vector2.up, null, null, true, true);
        }

        private void PunchSelfGun(GameObject gameObject)
        {
            this.data.healthHandler.TakeDamage(Vector2.up * (this.data.maxHealth * .20f), Vector2.up, null, null, true, true);
        }
    }
}
