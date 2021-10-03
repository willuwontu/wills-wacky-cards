using System.Collections.Generic;
using UnboundLib;

namespace WillsWackyCards.Utils
{
    public class CurseManager
    {
        public static List<CardInfo> curses = new List<CardInfo>();
        private static System.Random random = new System.Random();

        public static CardInfo RandomCurse => curses.ToArray()[random.Next(curses.Count)];
    }
}
