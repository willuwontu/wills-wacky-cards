using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using WWC.Interfaces;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    class Antidote : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;

            statModifiers.health = 1.5f;
            block.cdAdd = 0.5f;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.gameObject.AddComponent<Antidote_Mono>();
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.GetComponent<Antidote_Mono>();

            if (mono)
            {
                UnityEngine.GameObject.Destroy(mono);
            }
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Antidote";
        }
        protected override string GetDescription()
        {
            return "Blocking removes all poisons effecting you.";
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
                    positive = true,
                    stat = "HP",
                    amount = "+50%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Block CD",
                    amount = "+0.5s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Ability CD",
                    amount = "8s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.NatureBrown;
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
    class Antidote_Mono : MonoBehaviour, IPointStartHookHandler, IGameStartHookHandler
    {
        private float lastUsed;
        private float cooldown = 8f;

        private Player player;
        private CharacterData data;
        private Block block;
        private HealthHandler healthHandler;
        private DamageOverTime dot;

        private void Start()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            this.player = gameObject.GetComponentInParent<Player>();
            this.data = player.data;
            this.block = data.block;
            this.healthHandler = data.healthHandler;
            this.dot = ((DamageOverTime)this.healthHandler.GetFieldValue("dot"));

            this.block.BlockAction += this.OnBlock;
            this.healthHandler.reviveAction += this.OnRevive;
        }

        private void OnRevive()
        {
            this.lastUsed = 0f;
        }

        private void OnBlock(BlockTrigger.BlockTriggerType blockTrigger)
        {
            if (Time.time > this.lastUsed + this.cooldown)
            {
                this.lastUsed = Time.time;
                dot.StopAllCoroutines();
            }
        }

        public void OnPointStart()
        {
            this.lastUsed = 0f;
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
            this.block.BlockAction -= this.OnBlock;
            this.healthHandler.reviveAction -= this.OnRevive;
        }
    }
}