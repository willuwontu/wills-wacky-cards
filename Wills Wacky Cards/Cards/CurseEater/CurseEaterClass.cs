using ClassesManagerReborn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WWC.Cards
{
    internal class CurseEaterClass : ClassHandler
    {
        internal static string name = "Curse Eater";

        public override IEnumerator Init()
        {
            while (!(CorruptedAmmunition.card && CurseEater.card && Flagellation.card && GhostlyBody.card && HiltlessBlade.card && RunicWards.card && ShadowBullets.card && SiphonCurses.card)) yield return null;
            ClassesRegistry.Register(CurseEater.card, CardType.Entry);
            ClassesRegistry.Register(CorruptedAmmunition.card, CardType.Card | CardType.Branch, CurseEater.card);
            ClassesRegistry.Register(Flagellation.card, CardType.Card, CurseEater.card);
            ClassesRegistry.Register(GhostlyBody.card, CardType.Card | CardType.Branch, CurseEater.card);
            ClassesRegistry.Register(HiltlessBlade.card, CardType.Card, CurseEater.card);
            ClassesRegistry.Register(RunicWards.card, CardType.Card | CardType.Branch, CurseEater.card);
            ClassesRegistry.Register(ShadowBullets.card, CardType.Card | CardType.Branch, CurseEater.card);
            ClassesRegistry.Register(SiphonCurses.card, CardType.Card, CurseEater.card);
        }

        public override IEnumerator PostInit()
        {
            ClassesRegistry.Get(CorruptedAmmunition.card).Blacklist(ShadowBullets.card);
            ClassesRegistry.Get(ShadowBullets.card).Blacklist(CorruptedAmmunition.card);
            ClassesRegistry.Get(GhostlyBody.card).Blacklist(RunicWards.card);
            ClassesRegistry.Get(RunicWards.card).Blacklist(GhostlyBody.card);
            yield break;
        }
    }
}
