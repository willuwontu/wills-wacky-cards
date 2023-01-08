using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace WWC.Extensions
{
    public static class BlockExtension
    {
        public static void UpdateParticleDuration(this Block block)
        {
            var ratio = 1f / ((0.3f + block.GetComponent<CharacterStatModifiers>().GetAdditionalData().extraBlockTime) / 0.3f);

            var main = block.particle.main;
            main.simulationSpeed = ratio;
        }

        public static float BlocksPerSecond(this Block block) 
        {
            var blockCount = Mathf.Max(1, block.additionalBlocks + 1);
            var bps = blockCount / block.Cooldown();

            return bps;
        }
    }
}
