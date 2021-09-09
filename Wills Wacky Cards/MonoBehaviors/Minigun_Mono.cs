using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WillsWackyCards.Extensions;

namespace WillsWackyCards.MonoBehaviours
{
    public class Minigun_Mono : MonoBehaviour
    {
        public float heat = 0.0f;
        public float heatCap = 3.0f;
        private bool overheated = false;
        public float coolPerSecond = 2f;
        public float secondsBeforeStartToCool = 0.0f;
        private float cooldownTimeRemaining = 0.1f;
        private bool coroutineStarted;
        private float minigunDamageM = 0.035f;
        public float heatPerBullet = 0.01f;
        private Gun gun;
        private GunAmmo gunAmmo;
        private CharacterData data;
        private WeaponHandler weaponHandler;
        private Player player;

        // Heat Bar stuff, god bless Boss Sloth
        private GameObject heatBarObj;
        public Image heatImage;
        public Image whiteImage;
        private float heatTarget;
        //private HeatBar heatBar;

        private void Start()
        {
            data = GetComponentInParent<CharacterData>();
            heatBarObj = gameObject.transform.Find("WobbleObjects/HeatBar").gameObject;
            heatBarObj.transform.Find("Canvas/Image/Health").GetComponent<Image>().color = new Color(255,255,255);
            heatBarObj.transform.Find("Canvas/Image/Health").GetComponent<Image>().SetAlpha(1);
            heatImage = heatBarObj.transform.Find("Canvas/Image/Health").GetComponent<Image>();
            whiteImage = heatBarObj.transform.Find("Canvas/Image/White").GetComponent<Image>();
            whiteImage.SetAlpha(0);
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
                InvokeRepeating(nameof(Cooldown), 0, TimeHandler.deltaTime);
            }
            UpdateHeatBar();
        }

        private void UpdateHeatBar()
        {
            heatTarget = heat / heatCap;
            heatImage.fillAmount = heatTarget;
            whiteImage.fillAmount = heatTarget;
            if (!overheated)
            {
                heatImage.color = new Color(1f, 1f - (heatTarget) * 0.85f, 1f - heatTarget, 1f);
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
                cooldownTimeRemaining += 0.5f;
                heatImage.color = Color.red;
            }
        }
        private void Cooldown()
        {
            if (cooldownTimeRemaining > 0)
            {
                cooldownTimeRemaining -= TimeHandler.deltaTime;
            }
            if ((cooldownTimeRemaining <= 0) && (heat > 0f))
            {
                heat -= coolPerSecond * TimeHandler.deltaTime;
            }
            if (overheated && (heat <= 0f))
            {
                overheated = false;
                gun.GetAdditionalData().overHeated = false;
                gunAmmo.ReloadAmmo(true);
            }
        }
        private void OnDestroy()
        {
            Destroy(heatBarObj);
        }
    }
}