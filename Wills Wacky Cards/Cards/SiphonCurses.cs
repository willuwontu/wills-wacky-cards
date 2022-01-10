using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.MonoBehaviours;
using WillsWackyManagers.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    class SiphonCurses : CustomCard
    {
        internal static CardCategory siphonCard = CustomCardCategories.instance.CardCategory("Siphon Curse");
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CurseEater.CurseEaterClass, siphonCard};
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(20, () =>
            {
                int cursesToFind = 3;
                int cursesFound = 0;
                player.gameObject.GetOrAddComponent<SiphonCurses_Mono>();
                Dictionary<Player, List<int>> curseIndeces = new Dictionary<Player, List<int>>();
                List<Player> allyPlayers = PlayerManager.instance.players.Where((pl) => pl.playerID != player.playerID && pl.teamID == player.teamID).ToList();
                List<Player> enemyPlayers = PlayerManager.instance.players.Where((pl) => pl.playerID != player.playerID && pl.teamID != player.teamID).ToList();
                List<Player> otherPlayers = PlayerManager.instance.players.Where((pl) => pl.playerID != player.playerID).ToList();

                foreach (var person in otherPlayers.OrderBy((pl) => { if (allyPlayers.Contains(pl)) { return 0; } return 1; }).ToArray())
                {
                    curseIndeces.Add(person, (from index in Enumerable.Range(0, person.data.currentCards.Count()) where CurseManager.instance.IsCurse(person.data.currentCards[index]) select index).ToList());
                    cursesFound += curseIndeces[person].Count;
                    if (cursesFound >= cursesToFind)
                    {
                        break;
                    }
                }

                List<CardInfo> cursesToAdd = new List<CardInfo>();
                Dictionary<Player, List<int>> removedCurses = curseIndeces.ToDictionary((person) => person.Key, (person) => new List<int>());

                while (cursesToAdd.Count < cursesToFind)
                {
                    Player selectedPlayer;
                    if (curseIndeces.Keys.Intersect(allyPlayers).Count() > 0)
                    {
                        var options = curseIndeces.Keys.Intersect(allyPlayers).ToArray();
                        selectedPlayer = options[UnityEngine.Random.Range(0, options.Count())];

                        var randomIndex = curseIndeces[selectedPlayer][UnityEngine.Random.Range(0, curseIndeces[selectedPlayer].Count())];
                        curseIndeces[selectedPlayer].Remove(randomIndex);

                        var curse = selectedPlayer.data.currentCards[randomIndex];

                        cursesToAdd.Add(curse);
                        removedCurses[selectedPlayer].Add(randomIndex);

                        if (!(curseIndeces[selectedPlayer].Count > 0))
                        {
                            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][{GetTitle()}] {curseIndeces[selectedPlayer].Count} curses left to remove on player {selectedPlayer}. Removing them from possibilities.");
                            curseIndeces.Remove(selectedPlayer);
                        }

                        continue;
                    }
                    if (curseIndeces.Keys.Intersect(enemyPlayers).Count() > 0)
                    {
                        var options = curseIndeces.Keys.Intersect(enemyPlayers).ToArray();
                        selectedPlayer = options[UnityEngine.Random.Range(0, options.Count())];

                        var randomIndex = curseIndeces[selectedPlayer][UnityEngine.Random.Range(0, curseIndeces[selectedPlayer].Count())];
                        curseIndeces[selectedPlayer].Remove(randomIndex);

                        var curse = selectedPlayer.data.currentCards[randomIndex];

                        cursesToAdd.Add(curse);
                        removedCurses[selectedPlayer].Add(randomIndex);

                        if (!(curseIndeces[selectedPlayer].Count > 0))
                        {
                            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][{GetTitle()}] {curseIndeces[selectedPlayer].Count} curses left to remove on player {selectedPlayer}. Removing them from possibilities.");
                            curseIndeces.Remove(selectedPlayer);
                        }

                        continue;
                    }

                    UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][{GetTitle()}] Somehow we managed to break out the the loop, that should be impossible ...");
                    break;
                }

                foreach (var kvp in removedCurses)
                {
                    var removed = ModdingUtils.Utils.Cards.instance.RemoveCardsFromPlayer(kvp.Key, kvp.Value.ToArray());
                    ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(kvp.Key, removed);
                }

                ModdingUtils.Utils.Cards.instance.AddCardsToPlayer(player, cursesToAdd.ToArray(), false, null, null, null, true);
                ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, cursesToAdd.ToArray());

            });

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Siphon Curses";
        }
        protected override string GetDescription()
        {
            return "The darkness is your burden to bear alone. Steal 3 curses from other players, prioritizing teammates.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Move Speed",
                    amount = "+80%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Curses",
                    amount = "+3",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return WillsWackyCards.ModInitials;
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
