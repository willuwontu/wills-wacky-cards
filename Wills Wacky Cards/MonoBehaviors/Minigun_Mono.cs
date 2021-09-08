using UnityEngine;
using WillsWackyCards.Extensions;

namespace WillsWackyCards.MonoBehaviours
{
    public class Minigun_Mono : MonoBehaviour
    {
        public float heat = 0.0f;
        public float heatCap = 2.0f;
        private bool overheated = false;
        public float coolPerSecond = 0.4f;
        public float secondsBeforeStartToCool = 0.1f;
        private float cooldownTimeLeft = 0.1f;
        private Gun gun;
        private GunAmmo gunAmmo;
        private CharacterData data;
        private WeaponHandler weaponHandler;
        private Player player;
        //private HeatBar heatBar;

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
                }
            }

            InvokeRepeating(nameof(Blip), 0, TimeHandler.deltaTime);
        }

        private void Blip()
        {

        }
        private void CooldownStart()
        {

        }
        private void OnDestroy()
        {
            //Destroy(armorBarObj);
        }
    }
}