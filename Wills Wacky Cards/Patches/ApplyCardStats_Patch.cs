using UnityEngine;
using HarmonyLib;
using WillsWackyCards.Extensions;

namespace WillsWackyCards.Patches
{
    [HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
    class ApplyCardStats_Patch
    {
        static void Prefix(Player ___playerToUpgrade, Gun ___myGunStats)
        {
            var gun = ___playerToUpgrade.GetComponent<Holding>().holdable.GetComponent<Gun>();

            if (___myGunStats.GetAdditionalData().useForcedAttackSpeed)
            {
                gun.GetAdditionalData().useForcedAttackSpeed = ___myGunStats.GetAdditionalData().useForcedAttackSpeed;
                gun.defaultCooldown = ___myGunStats.forceSpecificAttackSpeed;
            }

            if (___myGunStats.GetAdditionalData().useForcedReloadSpeed)
            {
                gun.GetAdditionalData().useForcedReloadSpeed = ___myGunStats.GetAdditionalData().useForcedReloadSpeed;
                gun.GetAdditionalData().forcedReloadSpeed = ___myGunStats.GetAdditionalData().forcedReloadSpeed;
            }
            if (___myGunStats.GetAdditionalData().useAttacksPerAttack)
            {
                gun.GetAdditionalData().attacksPerAttack = Mathf.Clamp(___myGunStats.GetAdditionalData().attacksPerAttack,1,10);
                gun.GetAdditionalData().useAttacksPerAttack = ___myGunStats.GetAdditionalData().useAttacksPerAttack;
            }
            gun.GetAdditionalData().speedDamageMultiplier *= ___myGunStats.GetAdditionalData().speedDamageMultiplier;

            if (___myGunStats.GetAdditionalData().minigun)
            {
                gun.GetAdditionalData().minigun = ___myGunStats.GetAdditionalData().minigun;
            }
            if (___myGunStats.GetAdditionalData().useHeat)
            {
                gun.GetAdditionalData().useHeat = ___myGunStats.GetAdditionalData().useHeat;
            }
        }

        static void Postfix(Player ___playerToUpgrade)
        {
            var player = ___playerToUpgrade.GetComponent<Player>();
            var gun = ___playerToUpgrade.GetComponent<Holding>().holdable.GetComponent<Gun>();
            var gunAmmo = gun.GetComponentInChildren<GunAmmo>();
            var healthHandler = ___playerToUpgrade.GetComponent<HealthHandler>();
            var characterStatModifiers = player.GetComponent<CharacterStatModifiers>();

            if (((gun.attackSpeed * gun.attackSpeedMultiplier) < gun.GetAdditionalData().minimumAttackSpeed) && !gun.GetAdditionalData().useForcedAttackSpeed)
            {
                gun.attackSpeed = gun.GetAdditionalData().minimumAttackSpeed;
                gun.attackSpeedMultiplier = 1f;
            }

            if (((gun.reloadTime + gun.reloadTimeAdd) < gun.GetAdditionalData().minimumReloadSpeed) && !gun.GetAdditionalData().useForcedReloadSpeed)
            {
                gun.reloadTime = gun.GetAdditionalData().minimumReloadSpeed;
                gun.reloadTimeAdd = 0.0f;
                gunAmmo.reloadTimeMultiplier = 1.0f;
                gunAmmo.reloadTimeAdd = 0f;
            }

            if (gun.GetAdditionalData().useForcedAttackSpeed)
            {
                gun.attackSpeed = gun.defaultCooldown;
                gun.attackSpeedMultiplier = 1f;
            }

            if (gun.GetAdditionalData().useForcedReloadSpeed)
            {
                gunAmmo.reloadTimeMultiplier = 1.0f;
                gunAmmo.reloadTimeAdd = 0f;
                gun.reloadTime = gun.GetAdditionalData().forcedReloadSpeed;
                gun.reloadTimeAdd = 0.0f;
            }

            if (characterStatModifiers.GetAdditionalData().Vampire)
            {
                healthHandler.regeneration = 0f;
            }
        }
    }
}