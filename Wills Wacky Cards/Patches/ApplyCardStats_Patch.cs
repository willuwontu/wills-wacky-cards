using UnityEngine;
using HarmonyLib;
using WWC.Extensions;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
    class ApplyCardStats_Patch
    {
        static void Prefix(Player ___playerToUpgrade, Gun ___myGunStats, CharacterStatModifiers ___myPlayerStats)
        {
            var gun = ___playerToUpgrade.GetComponent<Holding>().holdable.GetComponent<Gun>();
            var statModifiers = ___playerToUpgrade.GetComponent<CharacterStatModifiers>();
            if (___myGunStats)
            {
                if (___myGunStats.GetAdditionalData().useMinimumReloadSpeed)
                {
                    gun.GetAdditionalData().useMinimumReloadSpeed = ___myGunStats.GetAdditionalData().useMinimumReloadSpeed;
                    gun.GetAdditionalData().minimumReloadSpeed = ___myGunStats.GetAdditionalData().minimumReloadSpeed;
                }
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
                    gun.GetAdditionalData().attacksPerAttack = Mathf.Clamp(___myGunStats.GetAdditionalData().attacksPerAttack, 1, 10);
                    gun.GetAdditionalData().useAttacksPerAttack = ___myGunStats.GetAdditionalData().useAttacksPerAttack;
                }
                gun.GetAdditionalData().speedDamageMultiplier *= ___myGunStats.GetAdditionalData().speedDamageMultiplier;
            }
            if (___myPlayerStats)
            {
                if (___myPlayerStats.GetAdditionalData().MassModifier != 1)
                {
                    statModifiers.GetAdditionalData().MassModifier *= ___myPlayerStats.GetAdditionalData().MassModifier;
                }
            }
        }

        static void Postfix(Player ___playerToUpgrade)
        {
            var player = ___playerToUpgrade.GetComponent<Player>();
            var gun = ___playerToUpgrade.GetComponent<Holding>().holdable.GetComponent<Gun>();
            var gunAmmo = gun.GetComponentInChildren<GunAmmo>();
            var healthHandler = ___playerToUpgrade.GetComponent<HealthHandler>();
            var characterStatModifiers = player.GetComponent<CharacterStatModifiers>();
            var characterData = ___playerToUpgrade.GetComponent<CharacterData>();

            if (((gun.attackSpeed * gun.attackSpeedMultiplier) < gun.GetAdditionalData().minimumAttackSpeed) && gun.GetAdditionalData().useMinimumAttackSpeed && !gun.GetAdditionalData().useForcedAttackSpeed)
            {
                gun.attackSpeed = gun.GetAdditionalData().minimumAttackSpeed;
                gun.attackSpeedMultiplier = 1f;
            }

            if (((gunAmmo.reloadTime + gunAmmo.reloadTimeAdd) * gunAmmo.reloadTimeMultiplier < gun.GetAdditionalData().minimumReloadSpeed) && gun.GetAdditionalData().useMinimumReloadSpeed && !gun.GetAdditionalData().useForcedReloadSpeed)
            {
                gun.reloadTime = 1f;
                gun.reloadTimeAdd = 0.0f;
                gunAmmo.reloadTimeMultiplier = 1.0f;
                gunAmmo.reloadTime = 2f;
                gunAmmo.reloadTimeAdd = gun.GetAdditionalData().minimumReloadSpeed - gunAmmo.reloadTime;
            }

            if (gun.GetAdditionalData().useForcedAttackSpeed)
            {
                gun.attackSpeed = gun.defaultCooldown;
                gun.attackSpeedMultiplier = 1f;
            }

            if (characterStatModifiers.GetAdditionalData().Vampire)
            {
                healthHandler.regeneration = 0f;
            }
        }
    }
}