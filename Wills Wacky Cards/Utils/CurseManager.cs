using System.Collections.Generic;
using System.Linq;
using UnboundLib;

namespace WillsWackyCards.Utils
{
    /// <summary>
    /// Manages the collection of curse cards given out, and contains various methods for utilizing them.
    /// </summary>
    public class CurseManager
    {
        private static List<CardInfo> curses = new List<CardInfo>();
        private static System.Random random = new System.Random();

        /// <summary>
        /// Returns a random curse from the list of curses, if one exists.
        /// </summary>
        public static CardInfo RandomCurse => curses.ToArray()[random.Next(curses.Count)];

        /// <summary>
        /// Adds the curse to the list of available curses for a player.
        /// The curse is automatically registered to ModdingUtils list of hidden cards as well.
        /// </summary>
        /// <param name="cardInfo">The card to register.</param>
        public static void RegisterCurse(CardInfo cardInfo)
        {
            curses.Add(cardInfo); 
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo);
        }

        /// <summary>
        /// Returns an array containing all curses available.
        /// </summary>
        public static CardInfo[] GetRaw()
        {
            return curses.ToArray();
        }
        
        /// <summary>
        /// Checks to see if a player has a curse.
        /// </summary>
        /// <param name="player">The player to check for a curse.</param>
        /// <returns>Returns true if a player has a registered curse.</returns>
        public static bool HasCurse(Player player)
        {
            bool result = false;

            result = curses.Intersect(player.data.currentCards).Any();

            return result;
        }
    }
}
