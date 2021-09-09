using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using WillsWackyCards.Extensions;

namespace WillsWackyCards.MonoBehaviours
{
    public class Minigun_Mono : MonoBehaviour
    {
        public float heat = 0.0f;
        public float heatCap = 1.0f;
        private bool overheated = false;
        public float coolPerSecond = 0.5f;
        public float secondsBeforeStartToCool = 0.1f;
        private float cooldownTimeRemaining = 0.1f;
        private bool coroutineStarted;
        private float minigunDamageM = 0.035f;
        public float heatPerBullet = 0.02f;
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
        private float whiteTarget;
        private float heatCurrent;
        private float whiteCurrent;
        private float whitedx;
        private float heatdx;
        private float drag = 25f;
        private float spring = 25f;
        private float sinceHeat = 0f;
        //private HeatBar heatBar;

        private void Start()
        {
            data = GetComponentInParent<CharacterData>();
            heatBarObj = gameObject.transform.Find("WobbleObjects/HeatBar").gameObject;
            heatBarObj.transform.Find("Canvas/Image/Health").GetComponent<Image>().color = Color.red * 0.6f;
            heatBarObj.transform.Find("Canvas/Image/Health").GetComponent<Image>().SetAlpha(1);
            heatImage = heatBarObj.transform.Find("Canvas/Image/Health").GetComponent<Image>();
            whiteImage = heatBarObj.transform.Find("Canvas/Image/White").GetComponent<Image>();
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
            if (coroutineStarted)
            {
                UpdateHeatBar();
            }
        }

        private void UpdateHeatBar()
        {
            heatTarget = heat / heatCap;
            sinceHeat += TimeHandler.deltaTime;
            heatdx = FRILerp.Lerp(heatdx, (heatTarget - heatCurrent) * spring, drag);
            whitedx = FRILerp.Lerp(whitedx, (whiteTarget - whiteCurrent) * spring, drag);
            heatCurrent += heatdx * TimeHandler.deltaTime;
            whiteCurrent += whitedx * TimeHandler.deltaTime;
            heatImage.fillAmount = heatCurrent;
            whiteImage.fillAmount = whiteCurrent;
            if (sinceHeat > 0.5)
            {
                whiteTarget = heatTarget;
            }
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            ProjectileHit bullet = obj.GetComponent<ProjectileHit>();
            UnityEngine.Debug.Log(string.Format("[WWC][Minigun] {0} heat", heat));
            bullet.damage *= minigunDamageM;
            heat += heatPerBullet;
            cooldownTimeRemaining = secondsBeforeStartToCool;
            sinceHeat = 0;
            if (heat < heatCap)
            {
                gunAmmo.ReloadAmmo(false); 
            }
            else
            {
                overheated = true;
                gun.GetAdditionalData().overHeated = true;
                cooldownTimeRemaining += 0.5f;
            }
        }

        public void SetupRound()
        {
            heat = 0f;
        }
        public void CleanupRound()
        {
            heat = 0f;
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
            //Destroy(armorBarObj);
        }
    }
}