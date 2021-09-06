using HarmonyLib;
using UnityEngine;
using WillsWackyCards.Extensions;

namespace WillsWackyCards.Patches
{
    [HarmonyPatch(typeof(Gun))] // Patching the getShootRotation method in the gun object.
    class Gun_Patch
    {
        // Prefix patch is run before the original method is run.
        // Type Bool so that we can tell Harmony to skip the original method if we set anything.
        //   bulletID, numOfProj, charge are all parameters of the original method and used for calculations.
        //   __instance grabs the triggering instance.
        //   ___forceShootDir grabs the forcShootDir field of our triggering instance.
        //   __result is the returned result of the method, AKA how we modify what value is returned.
        [HarmonyPrefix]
        [HarmonyPatch("getShootRotation")]
        static bool EvenSpread(Gun __instance, int bulletID, int numOfProj, float charge, Vector3 ___forceShootDir, ref Quaternion __result) 
        {
            if ((__instance.spread != 0.0f)&&(__instance.evenSpread != 0.0f)) // If the gun has spread and there's an even spread factor.
            {
                Vector3 vector = __instance.shootPosition.forward;
                if (___forceShootDir != Vector3.zero)
                {
                    vector = ___forceShootDir;
                }
                float d = __instance.multiplySpread * Mathf.Clamp(1f + charge * __instance.chargeSpreadTo, 0f, float.PositiveInfinity);
                float num = Random.Range(-__instance.spread, __instance.spread);
                num /= (1f + __instance.projectileSpeed * 0.5f) * 0.5f;
                // New Code start
                var even = bulletID * ((__instance.spread * 2) / (numOfProj - 1)) - __instance.spread; // Direction the bullet would point in, if the shots were spread evenly.
                even /= (1f + __instance.projectileSpeed * 0.5f) * 0.5f; // Modify by the same factor that spread is modified by
                num = even + (1.0f - Mathf.Clamp(__instance.evenSpread, 0.0f, 1.0f)) * (num - even); // Use evenness factor to determine how much we align with the random vs even spread.
                // New Code end
                vector += Vector3.Cross(vector, Vector3.forward) * num * d;
                __result = Quaternion.LookRotation(__instance.lockGunToDefault ? __instance.shootPosition.forward : vector);
                return false; // Skip the original method.
            }
            return true; // Run the original method.
        }

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
            ProjectileHit bullet = obj.GetComponent<ProjectileHit>();
            bullet.damage *= (1f + ((___projectileSpeed - 1f) * (__instance.GetAdditionalData().speedDamageMultiplier - 1f))) * (1f + ((___projectielSimulatonSpeed - 1f) * (__instance.GetAdditionalData().speedDamageMultiplier - 1f)));

            if (___reflects > 0)
            {
                RayHitReflect rayHitReflect = obj.GetComponent<RayHitReflect>();
                rayHitReflect.dmgM *= (1f + ((rayHitReflect.speedM - 1f) * (__instance.GetAdditionalData().speedDamageMultiplier - 1f)));
            }

            if (__instance.GetAdditionalData().useHeat)
            {
                //WeaponHandler weaponHandler = __instance.GetComponentInParent<WeaponHandler>();
                //Traverse.Create(weaponHandler).Field("heat").SetValue((float)Traverse.Create(weaponHandler).Field("heat").GetValue() + __instance.GetAdditionalData().heatPerShot);
            }
        }

        //[HarmonyPrefix]
        //[HarmonyPatch("BulletInit")]
        //static void MinigunMadness(Gun __instance, ref bool useAmmo)
        //{
        //    if (__instance.GetAdditionalData().minigun)
        //    {
        //        GunAmmo gunAmmo = __instance.GetComponentInChildren<GunAmmo>();
        //        Traverse.Create(gunAmmo).Field("currentAmmo").SetValue((int)Traverse.Create(gunAmmo).Field("currentAmmo").GetValue() + 1);
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