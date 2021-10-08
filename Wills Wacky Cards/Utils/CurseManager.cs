using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using System;

namespace WillsWackyCards.Utils
{
    /// <summary>
    /// Manages the collection of curse cards given out, and contains various methods for utilizing them.
    /// </summary>
    public class CurseManager : MonoBehaviour
    {
        /// <summary>
        /// Instanced version of the class for accessibility from within static functions.
        /// </summary>
        public static CurseManager instance { get; private set; }
        private List<CardInfo> curses = new List<CardInfo>();
        private List<CardInfo> activeCurses = new List<CardInfo>();
        private System.Random random = new System.Random();

        /// <summary>
        /// The card category for all curses.
        /// </summary>
        public CardCategory curseCategory = CustomCardCategories.instance.CardCategory("Curse");

        /// <summary>
        /// The card category for cards that interact with cursed players.
        /// </summary>
        public CardCategory curseInteractionCategory = CustomCardCategories.instance.CardCategory("Cursed");

        private void Start()
        {
            instance = this;
        }

        private void CheckCurses()
        {
            activeCurses = curses.Intersect(CardManager.cards.Values.ToArray().Where((card) => card.enabled).Select(card => card.cardInfo).ToArray()).ToList();
            foreach (var item in activeCurses)
            {
                UnityEngine.Debug.Log($"[WWC][Debugging] {item.cardName} is an enabled curse.");
            }
        }

        /// <summary>
        /// Returns a random curse from the list of curses, if one exists.
        /// </summary>
        /// <returns>CardInfo for the generated curse.</returns>
        public CardInfo RandomCurse()
        {
            CheckCurses();

            var choices = new CardChoice();

            choices.cards = activeCurses.ToArray();

            var curse = ((GameObject)choices.InvokeMethod("GetRanomCard")).GetComponent<CardInfo>();

            //return curse;
            return activeCurses.ToArray()[random.Next(activeCurses.Count)];
        }

        /// <summary>
        /// Curses a player with a random curse.
        /// </summary>
        /// <param name="player">The player to curse.</param>
        public void CursePlayer(Player player)
        {
            CursePlayer(player, null);
        }

        /// <summary>
        /// Curses a player with a random curse.
        /// </summary>
        /// <param name="player">The player to curse.</param>
        /// <param name="callback">An action to run with the information of the curse.</param>
        public void CursePlayer(Player player, Action<CardInfo> callback)
        {
            var curse = RandomCurse();
            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, curse, false, "", 2f, 2f, true);
            UnityEngine.Debug.Log($"[WWC][Curse Manager] Player {player.playerID} cursed with {curse.cardName}.");
            callback?.Invoke(curse);
        }

        /// <summary>
        /// Adds the curse to the list of available curses.
        /// </summary>
        /// <param name="cardInfo">The card to register.</param>
        public void RegisterCurse(CardInfo cardInfo)
        {
            curses.Add(cardInfo); 
            //ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo);
            //CheckCurses();
        }

        /// <summary>
        /// Returns an array containing all curses available.
        /// </summary>
        public CardInfo[] GetRaw()
        {
            return curses.ToArray();
        }
        
        /// <summary>
        /// Checks to see if a player has a curse.
        /// </summary>
        /// <param name="player">The player to check for a curse.</param>
        /// <returns>Returns true if a player has a registered curse.</returns>
        public bool HasCurse(Player player)
        {
            bool result = false;

            result = curses.Intersect(player.data.currentCards).Any();

            return result;
        }

        /// <summary>
        /// Checks to see if a card is a curse or not.
        /// </summary>
        /// <param name="cardInfo">The card to check.</param>
        /// <returns>Returns true if the card is a registered curse.</returns>
        public bool IsCurse(CardInfo cardInfo)
        {
            return curses.Contains(cardInfo);
        }
    }
}
