using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    class UnstoppableForce : CustomCard
    {
        public static CardInfo card = null;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            var tracker = this.gameObject.GetOrAddComponent<MomentumCard_Mono>();
            tracker.card = cardInfo;
            tracker.title = "Unstoppable Force";
            tracker.statsGenerator = GetStats;

            var stacks = MomentumTracker.stacks = Math.Max(MomentumTracker.stacks, 1);

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {stacks} Momentum Stacks built up");

            cardInfo.cardStats = MomentumTracker.GetOffensiveMomentumStats(stacks);
            tracker.updated = true;
            tracker.alwaysUpdate = true;

            cardInfo.categories = new CardCategory[] { WillsWackyManagers.Utils.RerollManager.instance.NoFlip, CustomCardCategories.instance.CardCategory("NoRandom") };

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var stacks = MomentumTracker.stacks;
            if (!MomentumTracker.createdOffenseCards.TryGetValue(stacks, out var cardData))
            {
                CustomCard.BuildCard<BuildUnstoppableForce>(cardInfo => {
                    MomentumTracker.createdOffenseCards.Add(stacks, cardInfo);
                    ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo);
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, cardInfo, false, "", 2f, 2f, true);
                });
            }
            else
            {
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, cardData, false, "", 2f, 2f, true);
            }

            WillsWackyCards.remover.DelayedRemoveCard(player, GetTitle(), 40, true);

            //var cleaner = player.gameObject.GetOrAddComponent<UnstoppableForceCleanup_Mono>();
            //cleaner.player = player;

            //cleaner.CleanUp();
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
                
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] Unstoppable Force base card removed from player {player.teamID}");
        }

        protected override string GetTitle()
        {
            return "Unstoppable Force";
        }
        protected override string GetDescription()
        {
            return "Stats increase each time it appears without being taken.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return Rarities.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return MomentumTracker.GetOffensiveMomentumStats(MomentumTracker.stacks);
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
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

    class BuildUnstoppableForce : UnstoppableForce
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            var tracker = this.gameObject.GetOrAddComponent<MomentumCard_Mono>();
            int stacks;
            if (!tracker.builtCard)
            {
                tracker.card = cardInfo;
                tracker.title = "Unstoppable Force";
                tracker.stacks = Math.Max(MomentumTracker.stacks, 1);
                tracker.builtCard = true;
                tracker.statsGenerator = GetStats;
            }
            stacks = tracker.stacks;

            gun.ammo = stacks;
            gun.attackSpeed = (float)Math.Pow(.96f, stacks);
            gun.projectileSpeed = (float)Math.Pow(1.04f, stacks);
            gun.reflects = stacks / 2;
            gun.damage = (float)Math.Pow(1.04f, stacks);
            gun.bursts = stacks / 10;
            gun.numberOfProjectiles = stacks / 5;
            gun.reloadTime = (float)Math.Pow(1.04f, stacks);

            cardInfo.cardStats = MomentumTracker.GetOffensiveMomentumStats(stacks);
            tracker.updated = true;

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var tracker = this.gameObject.GetOrAddComponent<MomentumCard_Mono>();
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] Unstoppable Force ({tracker.stacks} stacks) added to player {player.teamID}");

            //if (!(GM_Test.instance != null && GM_Test.instance.gameObject.activeInHierarchy))
            {
                MomentumTracker.stacks = MomentumTracker.stacks / 4;
                MomentumTracker.ResetRarityBuff();
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var tracker = this.gameObject.GetOrAddComponent<MomentumCard_Mono>();
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] Unstoppable Force ({tracker.stacks} stacks) removed from player {player.teamID}");
        }
        protected override string GetTitle()
        {
            return $"Unstoppable Force ({MomentumTracker.stacks} Stacks)";
        }
        public override bool GetEnabled()
        {
            return false;
        }
    }
    public class UnstoppableForceCleanup_Mono : MonoBehaviour
    {
        public Player player;
        public void CleanUp()
        {
            this.ExecuteAfterFrames(6, AddCard);
        }

        private void AddCard()
        {
            this.ExecuteAfterFrames(6, Clean);
        }

        private void Clean()
        {
            var cardCount = player.data.currentCards.Where(cardInfo => cardInfo.cardName == "Unstoppable Force").ToArray().Count();
            if (cardCount > 0)
            {
                var cards = player.data.currentCards;
                for (int i = cards.Count - 1; i >= 0; i--)
                {
                    if (cards[i].cardName == "Unstoppable Force")
                    {
                        ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, i);
                        break;
                    }
                }
            }
        }
    }
}
