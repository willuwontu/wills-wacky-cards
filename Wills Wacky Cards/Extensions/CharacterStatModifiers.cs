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
        public int shieldsRemaining;

        public CharacterStatModifiersAdditionalData()
        {
            Vampire = false;
            MassModifier = 1f;
            Bleed = 0f;
            shieldsRemaining = 0;
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
    [HarmonyPatch(typeof(CharacterStatModifiers), "ResetStats")]
    class CharacterStatModifiersPatchResetStats
    {
        private static void Prefix(CharacterStatModifiers __instance)
        {
            __instance.GetAdditionalData().Vampire = false;
            __instance.GetAdditionalData().MassModifier = 1f;
            __instance.GetAdditionalData().Bleed = 0f;
            __instance.GetAdditionalData().shieldsRemaining = 0;
        }
    }
}
