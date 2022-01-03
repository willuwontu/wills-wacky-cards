using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.GameModes;
using UnboundLib.Cards;
using WWC.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards.Testing
{
    class ResetTeamScores : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            foreach (var person in PlayerManager.instance.players)
            {
                var score = GameModeManager.CurrentHandler.GetTeamScore(person.teamID);
                GameModeManager.CurrentHandler.SetTeamScore(person.teamID, new TeamScore(0, 0));
            }

            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");

            RemoveTestingCards(player);
        }

        protected override string GetTitle()
        {
            return "Reset Team Scores";
        }
        protected override string GetDescription()
        {
            return "Resets the score for all teams.";
        }
    }
    class AddRoundToTeam : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var score = GameModeManager.CurrentHandler.GetTeamScore(player.teamID);
            GameModeManager.CurrentHandler.SetTeamScore(player.teamID, new TeamScore(score.points, score.rounds + 1));
            score = GameModeManager.CurrentHandler.GetTeamScore(player.teamID);
            UnityEngine.Debug.Log($"Team {player.teamID} now has a total of {score.rounds} rounds.");
            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");

            RemoveTestingCards(player);
        }

        protected override string GetTitle()
        {
            return "Add Round to Team";
        }
        protected override string GetDescription()
        {
            return "Adds a round to the player's team's score.";
        }
    }
    class AddPointToTeam : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var score = GameModeManager.CurrentHandler.GetTeamScore(player.teamID);
            GameModeManager.CurrentHandler.SetTeamScore(player.teamID, new TeamScore(score.points + 1, score.rounds));
            score = GameModeManager.CurrentHandler.GetTeamScore(player.teamID);
            UnityEngine.Debug.Log($"Team {player.teamID} now has a total of {score.points} Points.");
            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");

            RemoveTestingCards(player);
        }

        protected override string GetTitle()
        {
            return "Add Point to Team";
        }
        protected override string GetDescription()
        {
            return "Adds a point to the player's team's score.";
        }
    }
}
