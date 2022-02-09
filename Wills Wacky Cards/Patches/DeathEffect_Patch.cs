using HarmonyLib;
using UnityEngine;
using WWC.Extensions;
using WWC.MonoBehaviours;
using Sonigon;
using UnboundLib;

namespace WWC.Patches
{
    [HarmonyPatch(typeof(DeathEffect))] 
    class DeathEffect_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DeathEffect.PlayDeath))]
        static void AdjustedRespawnTime(DeathEffect __instance, Color color, PlayerVelocity playerRig, Vector2 vel, int playerIDToRevive)
        {
            if (playerIDToRevive != -1)
            {
                var player = ((CharacterData) playerRig.GetFieldValue("data")).player;

                if (player.data.stats.GetAdditionalData().useNewRespawnTime)
                {
                    var newTime = player.data.stats.GetAdditionalData().newRespawnTime;                    
                    __instance.SetFieldValue("respawnTime", newTime);
                    var particles = __instance.GetComponentsInChildren<ParticleSystem>();

                    foreach (var particle in particles)
                    {
                        particle.playbackSpeed = 2.53f / newTime;
                        var main = particle.main;
                        main.simulationSpeed = 2.53f / newTime;
                    }

                    __instance.gameObject.GetComponent<RemoveAfterSeconds>().seconds = 2 * newTime;
                }
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