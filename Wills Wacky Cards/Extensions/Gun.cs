using System;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace WillsWackyCards.Extensions
{
    // ADD FIELDS TO GUN
    [Serializable]
    public class GunAdditionalData
    {
        public bool useForcedAttackSpeed;
        public float forcedReloadSpeed;
        public bool useForcedReloadSpeed;
        public bool useAttacksPerAttack;
        public int attacksPerAttack;
        public bool shotgun;
        public float minimumAttackSpeed;
        public float minimumReloadSpeed;
        public float speedDamageMultiplier;
        public bool useHeat;
        public float heatPerShot;
        public bool minigun;

        public GunAdditionalData()
        {
            useForcedAttackSpeed = false;
            forcedReloadSpeed = 0.0f;
            useForcedReloadSpeed = false;
            attacksPerAttack = 1;
            useAttacksPerAttack = false;
            shotgun = false;
            speedDamageMultiplier = 1f;
            useHeat = false;
            heatPerShot = 0f;
            minigun = false;
        }
    }
    public static class GunExtension
    {
        public static readonly ConditionalWeakTable<Gun, GunAdditionalData> data =
            new ConditionalWeakTable<Gun, GunAdditionalData>();

        public static GunAdditionalData GetAdditionalData(this Gun gun)
        {
            return data.GetOrCreateValue(gun);
        }

        public static void AddData(this Gun gun, GunAdditionalData value)
        {
            try
            {
                data.Add(gun, value);
            }
            catch (Exception) { }
        }
    }
    // reset extra gun attributes when resetstats is called
    [HarmonyPatch(typeof(Gun), "ResetStats")]
    class GunPatchResetStats
    {
        private static void Prefix(Gun __instance)
        {
            __instance.GetAdditionalData().useForcedAttackSpeed = false;
            __instance.GetAdditionalData().useForcedReloadSpeed = false;
            __instance.GetAdditionalData().forcedReloadSpeed = 0.0f;
            __instance.GetAdditionalData().useAttacksPerAttack = false;
            __instance.GetAdditionalData().attacksPerAttack = 1;
            __instance.GetAdditionalData().shotgun = false;
            __instance.GetAdditionalData().speedDamageMultiplier = 1f;
            __instance.GetAdditionalData().heatPerShot = 0f;
            __instance.GetAdditionalData().useHeat = false;
            __instance.GetAdditionalData().minigun = false;
        }
    }
}
