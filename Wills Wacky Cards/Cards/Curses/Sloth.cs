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
using WillsWackyManagers.UnityTools;
using Photon.Realtime;
using UnboundLib.Utils;

namespace WWC.Cards.Curses
{
    class Sloth : CustomCard, ICurseCard, IConditionalCard
    {
        internal static CardInfo card;
        public CardInfo Card { get => card; set { if (!card) { card = value; } } }


        public bool Condition(Player player, CardInfo card)
        {
            if (card == null) 
            { 
                return true; 
            }
            if (!player || !player.data || player.data.currentCards == null)
            {
                return true;
            }

            if (!(player.data.currentCards.Contains(Sloth.card)))
            {
                return true;
            }

            List<CardInfo> cards = new List<CardInfo>();

            foreach (Player person in PlayerManager.instance.players)
            {
                if (person != player)
                {
                    cards.AddRange(person.data.currentCards);
                }
            }

            cards = cards.Distinct().ToList();

            if (!cards.Contains(card))
            {
                return false;
            }

            return true;
        }

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseCategory };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Sloth";
        }
        protected override string GetDescription()
        {
            return "Ugh, I don't want to go back and forth, can't I just stay here?";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return Rarities.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {

            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CurseManager.instance.CurseGray;
        }
        public override string GetModName()
        {
            return WillsWackyCards.CurseInitials;
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
