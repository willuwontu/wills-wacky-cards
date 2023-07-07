using System;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace WWC.Extensions
{
    // ADD FIELDS TO GUN
    [Serializable]
    public class CharacterStatModifiersAdditionalData
    {
        public bool Vampire;
        public float MassModifier;
        public float Bleed;
        public float DamageReduction;
        public bool isBanished;
        public float willpower;
        public float extraBlockTime;
        public bool useNewRespawnTime;
        public float newRespawnTime;
        public float poisonResistance;
        public float poisonDurationModifier;
        public float poisonBurstModifier;
        public float dealtDoTBurstModifier;
        public WWCNullData nullData;

        public CharacterStatModifiersAdditionalData()
        {
            Vampire = false;
            MassModifier = 1f;
            Bleed = 0f;
            DamageReduction = 0f;
            isBanished = false;
            willpower = 0f;
            extraBlockTime = 0f;
            useNewRespawnTime = false;
            newRespawnTime = 0f;
            poisonResistance = 1f;
            poisonBurstModifier = 1f;
            dealtDoTBurstModifier = 1f;
            poisonDurationModifier = 1f;
            nullData = new WWCNullData();
        }
    }
    public static class CharacterStatModifiersExtension
    {
        public static readonly ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersAdditionalData> data =
            new ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersAdditionalData>();

        public static CharacterStatModifiersAdditionalData GetAdditionalData(this CharacterStatModifiers statModifiers)
        {
            return data.GetOrCreateValue(statModifiers);
        }

        public static void AddData(this CharacterStatModifiers statModifiers, CharacterStatModifiersAdditionalData value)
        {
            try
            {
                data.Add(statModifiers, value);
            }
            catch (Exception) { }
        }
    }

    public class WWCNullData
    {
        public float willPowerAdd = 1f;
        public float poisonResMult = 1f;
        public int damageRedCards = 0;
    }

    [HarmonyPatch(typeof(CharacterStatModifiers), "ResetStats")]
    class CharacterStatModifiersPatchResetStats
    {
        private static void Prefix(CharacterStatModifiers __instance)
        {
            __instance.GetAdditionalData().Vampire = false;
            __instance.GetAdditionalData().MassModifier = 1f;
            __instance.GetAdditionalData().Bleed = 0f;
            __instance.GetAdditionalData().DamageReduction = 0f;
            __instance.GetAdditionalData().isBanished = false;
            __instance.GetAdditionalData().willpower = 0f;
            __instance.GetAdditionalData().extraBlockTime = 0f;
            __instance.GetAdditionalData().useNewRespawnTime = false;
            __instance.GetAdditionalData().newRespawnTime = 0f;
            __instance.GetAdditionalData().poisonResistance = 1f;
            __instance.GetAdditionalData().poisonBurstModifier = 1f;
            __instance.GetAdditionalData().dealtDoTBurstModifier = 1f;
            __instance.GetAdditionalData().poisonDurationModifier = 1f;
            __instance.GetAdditionalData().nullData = new WWCNullData();
        }
    }
}
