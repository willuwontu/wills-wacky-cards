using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(DamageOverTime))] 
    class DamageOverTime_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(DamageOverTime.TakeDamageOverTime))]
        static void DurationChange(DamageOverTime __instance, Vector2 damage, Vector2 position, ref float time, float interval, Player damagingPlayer, CharacterData ___data)
        {
            var player = ___data.player;

            if (player.data.stats.GetAdditionalData().poisonDurationModifier != 1f)
            {
                time *= player.data.stats.GetAdditionalData().poisonDurationModifier;
            }
        }

        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(nameof(DamageOverTime.TakeDamageOverTime))]
        static bool RevertToBurst(DamageOverTime __instance, ref Vector2 damage, Vector2 position, float time, float interval, Color color, GameObject damagingWeapon, Player damagingPlayer, bool lethal, CharacterData ___data)
        {
            var player = ___data.player;
            float burstModifier = 1f;

            if (player.data.stats.GetAdditionalData().poisonBurstModifier != 1f)
            {
                burstModifier *= player.data.stats.GetAdditionalData().poisonBurstModifier;
            }

            if (damagingPlayer && damagingPlayer.data.stats.GetAdditionalData().dealtDoTBurstModifier != 1f)
            {
                burstModifier *= damagingPlayer.data.stats.GetAdditionalData().dealtDoTBurstModifier;
            }

            if (burstModifier < 1f)
            {
                float initialDamage = damage.magnitude;
                float initialTime = time;
                float initialInterval = interval;

                //UnityEngine.Debug.Log($"Initially intended to deal {string.Format("{0:F2}", initialDamage)} damage in {string.Format("{0:F2}", initialInterval)}s intervals over {string.Format("{0:F2}", initialTime)} seconds.");

                float burstDamage = initialDamage * (1f - Mathf.Clamp(burstModifier, 0f, 1f));

                //UnityEngine.Debug.Log($"Dealing {string.Format("{0:F2}", burstDamage)} damage as a burst.");

                player.data.healthHandler.DoDamage(damage.normalized * burstDamage, position, color, damagingWeapon, damagingPlayer, false, lethal, true);

                float finalDamage = initialDamage - burstDamage;

                //UnityEngine.Debug.Log($"Now only scheduled to deal {string.Format("{0:F2}", finalDamage)} damage in {string.Format("{0:F2}", initialInterval)}s intervals over {string.Format("{0:F2}", initialTime)} seconds.");

                if (finalDamage <= 0f)
                {
                    return false;
                }

                damage = damage.normalized * (finalDamage);
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(DamageOverTime.TakeDamageOverTime))]
        static void DoTDamageReduction(DamageOverTime __instance, ref Vector2 damage, Vector2 position, float time, float interval, Player damagingPlayer, CharacterData ___data)
        {
            var player = ___data.player;

            if (player.data.stats.GetAdditionalData().poisonResistance != 1f)
            {
                float damageMag = damage.magnitude;
                damageMag *= player.data.stats.GetAdditionalData().poisonResistance;
                Vector2 damageDir = damage.normalized;

                damage = damageDir * damageMag;
            }
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