using ClassesManagerReborn;
using System;
using System.Collections.Generic;
using System.Text;

namespace WWC.Cards
{
    internal class MechanicClass : ClassHandler
    {
        internal static string name = "Mechanic";

        public override System.Collections.IEnumerator Init()
        {
            while (!(ChemicalAmmunition.Card && CloningTanks.Card && CuttingLaser.Card && GreyGoo.Card && GyroscopicStabilizers.Card && ImpactDissipators.Card && ImprovedCycling.Card && ImprovedShieldCapacitors.Card && IntegratedTargeting.Card && JumpBoots.Card && Mechanic.Card && Omnitool.Card && ParticleWaveSequencer.Card && PersonalHammerspace.Card && PortableFabricator.Card)) yield return null;
            ClassesRegistry.Register(Mechanic.Card, CardType.Entry);
            ClassesRegistry.Register(ChemicalAmmunition.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(CloningTanks.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(CuttingLaser.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(GreyGoo.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(GyroscopicStabilizers.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(ImpactDissipators.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(ImprovedCycling.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(ImprovedShieldCapacitors.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(IntegratedTargeting.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(JumpBoots.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(Omnitool.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(ParticleWaveSequencer.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(PersonalHammerspace.Card, CardType.Card, Mechanic.Card);
            ClassesRegistry.Register(PortableFabricator.Card, CardType.Card, Mechanic.Card);
        }
    }
}
