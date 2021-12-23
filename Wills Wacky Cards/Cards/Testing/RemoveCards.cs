using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards.Testing
{
    class RemoveFirst : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(20,
                () => {
                    int[] indices = new int[2];

                    indices[0] = 0;
                    indices[1] = player.data.currentCards.Count - 1;

                    ModdingUtils.Utils.Cards.instance.RemoveCardsFromPlayer(player, indices);
                });
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        protected override string GetTitle()
        {
            return "Remove First";
        }
        protected override string GetDescription()
        {
            return "Removes the first card picked and this one.";
        }
    }
    class RemoveLast : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(20, 
                () => {
                    int[] indices = new int[2];

                    indices[0] = player.data.currentCards.Count - 2;
                    indices[1] = player.data.currentCards.Count - 1;

                    ModdingUtils.Utils.Cards.instance.RemoveCardsFromPlayer(player, indices);
                });
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        protected override string GetTitle()
        {
            return "Remove Last";
        }
        protected override string GetDescription()
        {
            return "Removes the last card picked and this one.";
        }
    }
    class RemoveAll : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(20, () => ModdingUtils.Utils.Cards.instance.RemoveAllCardsFromPlayer(player, true));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        protected override string GetTitle()
        {
            return "Remove All";
        }
        protected override string GetDescription()
        {
            return "Removes all cards on the player, including this one.";
        }
    }
    class RemoveTestingCards : TestingCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(40, () => {
                List<CardInfo> testCards = new List<CardInfo>();
                foreach (var card in player.data.currentCards)
                {
                    if (card.categories.Contains(WillsWackyCards.TestCardCategory))
                    {
                        testCards.Add(card);
                    }
                }

                ModdingUtils.Utils.Cards.instance.RemoveCardsFromPlayer(player, testCards.ToArray(), ModdingUtils.Utils.Cards.SelectionType.All , true);
            });
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Remove Testing Cards";
        }
        protected override string GetDescription()
        {
            return "Removes all testing cards, including this one.";
        }
    }
}
