using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using ModdingUtils.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using WillsWackyManagers.Utils;
using UnityEngine;
using System.Collections;
using static UnityEngine.EventSystems.EventTrigger;

namespace WWC.Cards
{
    class RitualisticSacrifice : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseInteractionCategory, RerollManager.instance.NoFlip };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.StartCoroutine(ReassignCurses(player));
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        public IEnumerator ReassignCurses(Player player)
        {
            for (int i = 0; i < 40; i++)
            {
                yield return null;
            }

            var curses = CurseManager.instance.GetAllCursesOnPlayer(player);
            CurseManager.instance.RemoveAllCurses(player);

            var enemies = PlayerManager.instance.players.Where((person) => person.teamID != player.teamID).ToArray();

            foreach (var curse in curses)
            {
                List<Player> validPlayers = new List<Player>();
                foreach (var enemy in enemies) 
                {
                    if (CurseManager.instance.PlayerIsAllowedCurse(enemy, curse))
                    {
                        validPlayers.Add(enemy);
                    }
                }

                if (validPlayers.Count > 0) 
                {
                    validPlayers.Shuffle();
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(validPlayers.First(), curse, false, "", 2f, 2f, true);
                }
                yield return null;
                yield return null;
                yield return null;
            }

            yield break;
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Drives me crazy
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Ritualistic Sacrifice";
        }
        protected override string GetDescription()
        {
            return "Let the blood of my foes bear the weight of my own.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Curses Passed to Foes",
                    amount = "All",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.PoisonGreen;
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
