using System;
using System.Collections.Generic;
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
    class DimensionalShuffle : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            statModifiers.health = 0.7f;
            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<DimensionalShuffle_Mono>();
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<DimensionalShuffle_Mono>();
            UnityEngine.GameObject.Destroy(mono);
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Dimensional Shuffle";
        }
        protected override string GetDescription()
        {
            return "When you block, each player's position is randomly swapped to another's.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "HP",
                    amount = "-30%",
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

namespace WWC.MonoBehaviours
{
    [DisallowMultipleComponent]
    public class DimensionalShuffle_Mono : Hooked_Mono
    {
        private CharacterData data;
        private Player player;
        private Block block;

        private void Start()
        {
            HookedMonoManager.instance.hookedMonos.Add(this);
            data = GetComponentInParent<CharacterData>();
            player = data.player;
            block = data.block;
            block.BlockAction += OnBlock;
        }

        private void OnBlock(BlockTrigger.BlockTriggerType blockTrigger)
        {
            var livingPlayers = PlayerManager.instance.players.Where((person) => !person.data.dead).ToArray();
            var playerPositions = livingPlayers.Select((person) => person.transform.position).ToList();

            foreach (var person in livingPlayers)
            {
                var index = UnityEngine.Random.Range(0, playerPositions.Count);
                person.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                person.transform.position = playerPositions[index];

                playerPositions.RemoveAt(index);
            }
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            block.BlockAction -= OnBlock;
            HookedMonoManager.instance.hookedMonos.Remove(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}