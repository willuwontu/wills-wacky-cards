using ClassesManagerReborn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WWC.Cards.CurseEater
{
    internal class CurseEaterClass : ClassHandler
    {
        internal static string name = "Curse Eater";

        public override IEnumerator Init()
        {
            while (!(CorruptedAmmunition.Card && CurseEater.Card && Flagellation.Card && GhostlyBody.Card && HiltlessBlade.Card && RunicWards.Card && ShadowBullets.Card && SiphonCurses.Card)) yield return null;
            ClassesRegistry.Register(CurseEater.Card, CardType.Entry);
            ClassesRegistry.Register(CorruptedAmmunition.Card, CardType.Card, CurseEater.Card);
            ClassesRegistry.Register(Flagellation.Card, CardType.Card, CurseEater.Card);
            ClassesRegistry.Register(GhostlyBody.Card, CardType.Card, CurseEater.Card);
            ClassesRegistry.Register(HiltlessBlade.Card, CardType.Card, CurseEater.Card);
            ClassesRegistry.Register(RunicWards.Card, CardType.Card, CurseEater.Card);
            ClassesRegistry.Register(ShadowBullets.Card, CardType.Card, CurseEater.Card);
            ClassesRegistry.Register(SiphonCurses.Card, CardType.Card, CurseEater.Card);
        }

        public override IEnumerator PostInit()
        {
            ClassesRegistry.Get(CorruptedAmmunition.Card).Blacklist(ShadowBullets.Card);
            ClassesRegistry.Get(ShadowBullets.Card).Blacklist(CorruptedAmmunition.Card);
            ClassesRegistry.Get(GhostlyBody.Card).Blacklist(RunicWards.Card);
            ClassesRegistry.Get(RunicWards.Card).Blacklist(GhostlyBody.Card);
            yield break;
        }
    }
}
