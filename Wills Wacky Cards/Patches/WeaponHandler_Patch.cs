using HarmonyLib;
using UnityEngine;
using WillsWackyCards.Extensions;
using WillsWackyCards.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WillsWackyCards.Patches
{
    [HarmonyPatch(typeof(WeaponHandler))] 
    class WeaponHandler_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Attack")]
        static bool ChargeAttack(WeaponHandler __instance, CharacterData ___data, GeneralInput ___input, bool ___soundFireHold)
        {
            var weaponHandler = __instance;
            if (weaponHandler.gun.useCharge)
            {
                var data = ___data;
                var gun = weaponHandler.gun;
                var gunAmmo = gun.GetComponentInChildren<GunAmmo>();
                var input = ___input;
                var player = data.player;
                var plasmaWeapon = player.GetComponent<PlasmaWeapon_Mono>();

                if (!plasmaWeapon)
                {
                    return false;
                }
                if (!gun)
                {
                    return false;
                }
                if (!gun.IsReady(0f))
                {
                    return false;
                }
                //if (input.shootIsPressed)
                //{
                //    if (!___soundFireHold)
                //    {
                //        ___soundFireHold = true;
                //        if (gun.isReloading || data.isSilenced)
                //        {
                            
                //        }
                //    }
                //}
                //else
                //{
                //    ___soundFireHold = false;
                //}
                //if (weaponHandler.gun.bursts == 0 && (!___soundFireHold || weaponHandler.gun.isReloading || data.isSilenced))
                //{
                //    weaponHandler.gun.soundGun.StopAutoPlayTail();
                //}

                if (input.shootWasPressed && (int) gunAmmo.GetFieldValue("currentAmmo") > 0)
                {
                    gun.GetAdditionalData().beginCharge = true;
                    UnityEngine.Debug.Log("[WWC][Plasma Weapon] Beginning to charge shot.");
                }
                else if (input.shootWasPressed)
                {
                    SoundManager.Instance.Play(weaponHandler.soundCharacterCantShoot, weaponHandler.transform);
                }

                if (input.shootIsPressed)
                {
                    if (gun.GetAdditionalData().beginCharge && gun.currentCharge < 1f)
                    {
                        gun.currentCharge = Mathf.Clamp(gun.currentCharge + TimeHandler.deltaTime / gun.GetAdditionalData().chargeTime, 0f, 1f);
                        //UnityEngine.Debug.Log(string.Format("[WWC][Plasma Rifle] Gun is currently {0:F1}% charged.", gun.currentCharge * 100f)); 
                    }
                }
                if (input.shootWasReleased)
                {
                    if (gun.GetAdditionalData().beginCharge)
                    {
                        UnityEngine.Debug.Log(string.Format("[WWC][Plasma Weapon] Charged shot was fired at {0:F1}% charge.", gun.currentCharge * 100f));
                        plasmaWeapon.chargeToUse = gun.currentCharge;
                        weaponHandler.gun.Attack(gun.currentCharge, false, gun.currentCharge * gun.chargeDamageMultiplier, 1f, true);
                        gun.currentCharge = 0f;
                        gun.GetAdditionalData().beginCharge = false; 
                    }
                }
                return false;
            }
            return true;
        }

        //[HarmonyPrefix]
        //[HarmonyPatch("SomeMethod")]
        //static void MyMethodName()
        //{

        //}

        //[HarmonyPostfix]
        //[HarmonyPatch("SomeMethod")]
        //static void MyMethodName()
        //{

        //}
    }
}