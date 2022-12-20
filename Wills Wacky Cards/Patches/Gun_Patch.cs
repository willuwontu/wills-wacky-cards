using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(Gun))] 
    class Gun_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("DoAttacks")]
        static void SetAttacks(Gun __instance, ref int attacks)
        {
            if (__instance.GetAdditionalData().useAttacksPerAttack)
            {
                attacks = __instance.GetAdditionalData().attacksPerAttack;
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch("ApplyProjectileStats")]
        static void SpeedDamageIncrease(Gun __instance, ref GameObject obj, float ___projectileSpeed, int ___reflects, float ___projectielSimulatonSpeed)
        {
            if (__instance.GetAdditionalData().speedDamageMultiplier != 0f)
            {
                ProjectileHit bullet = obj.GetComponent<ProjectileHit>();

                var bulletSpeed = ___projectileSpeed * (__instance.useCharge ? ((GunChargePatch.Extensions.ProjectileHitExtensions.GetBulletCharge(bullet) * __instance.chargeSpeedTo)) : 1f);
                float speedMult = (1f + ((bulletSpeed - 1f) * (__instance.GetAdditionalData().speedDamageMultiplier - 1f)));
                float simulationMult = (1f + ((___projectielSimulatonSpeed - 1f) * (__instance.GetAdditionalData().speedDamageMultiplier - 1f)));

                //float extraDamage = (bullet.damage * (1f + ((___projectileSpeed - 1f) * (__instance.GetAdditionalData().speedDamageMultiplier - 1f))) * (1f + ((___projectielSimulatonSpeed - 1f) * (__instance.GetAdditionalData().speedDamageMultiplier - 1f)))) - bullet.damage;
                bullet.damage *= speedMult * simulationMult;
                //UnityEngine.Debug.Log(string.Format("[WWC] Bullet fired that deals {0} damage ({1} extra)", bullet.damage, extraDamage)); 
            }

            if (___reflects > 0)
            {
                RayHitReflect rayHitReflect = obj.GetComponent<RayHitReflect>();
                rayHitReflect.dmgM *= (1f + ((rayHitReflect.speedM - 1f) * (__instance.GetAdditionalData().speedDamageMultiplier - 1f)));
            }
        }

        //[HarmonyPostfix]
        //[HarmonyPatch("ApplyProjectileStats")]
        //static void MinigunHeat(Gun __instance)
        //{
        //    if (__instance.GetAdditionalData().useHeat)
        //    {
        //        WeaponHandler weaponHandler = __instance.GetComponentInParent<WeaponHandler>();
        //        Player player = __instance.player;
        //        //weaponHandler.SetFieldValue("heat",(float)weaponHandler.GetFieldValue("heat")+ __instance.GetAdditionalData().heatPerShot);
        //    }
        //}

        //[HarmonyPrefix]
        //[HarmonyPatch("BulletInit")]
        //static void MinigunMadness(Gun __instance, ref bool useAmmo)
        //{
        //    if (__instance.GetAdditionalData().minigun)
        //    {
        //        GunAmmo gunAmmo = __instance.GetComponentInChildren<GunAmmo>();
        //        gunAmmo.SetFieldValue("currentAmmo", (int)gunAmmo.GetFieldValue("currentAmmo") + 1);
        //    }
        //}

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