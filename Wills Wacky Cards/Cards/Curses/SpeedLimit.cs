using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using WillsWackyManagers.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using WillsWackyManagers.UnityTools;
using WWC.Interfaces;

namespace WWC.Cards.Curses
{
    class SpeedLimit : CustomCard, ICurseCard, IConditionalCard
    {
        private static CardInfo card;
        public CardInfo Card { get => card; set { if (!card) { card = value; } } }
        public bool Condition(Player player, CardInfo card)
        {
            if (card != SpeedLimit.card)
            {
                return true;
            }

            if (!player || !player.data || !player.data.block || (player.data.currentCards == null))
            {
                return true;
            }

            if (player.data.currentCards.Select(c => c.cardName.ToLower()).Intersect(new string[] { 
                "refresh", 
                "blood magic", 
                "shields up", 
                "hungry block", 
                "abyssal countdown", 
                "octablock" 
            }).Count() > 0)
            {
                return true;
            }

            if (player.data.block.BlocksPerSecond() < 3f)
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
            player.gameObject.AddComponent<SpeedLimitMono>();
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<SpeedLimitMono>();
            UnityEngine.GameObject.Destroy(mono);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Slow Reflexes";
        }
        protected override string GetDescription()
        {
            return "Constantly defending yourself takes its toll.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return Rarities.Trinket;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {

            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CurseManager.instance.FrozenBlue;
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

namespace WWC.MonoBehaviours
{
    public class SpeedLimitMono : MonoBehaviour
    {
        Player player;

        private void Awake()
        {
            this.player = GetComponentInParent<Player>();
            this.player.data.block.BlockAction += OnBlock;
        }

        private void OnBlock(BlockTrigger.BlockTriggerType blockTrigger)
        {
            this.player.gameObject.AddComponent<SpeedLimitTimeScaleMono>().duration += 10f;
        }

        private void OnDestroy()
        {
            this.player.data.block.BlockAction -= OnBlock;
        }
    }

    public class SpeedLimitTimeScaleMono : PlayerTimeScale.PlayerTimeScale, IPointEndHookHandler, IPointStartHookHandler, IGameStartHookHandler
    {
        public float duration;

        private void Awake()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            this.Scale = 0.9f;
        }

        private void Update()
        {
            if (duration < 0) 
            {
                UnityEngine.GameObject.Destroy(this);
            }

            duration -= TimeHandler.deltaTime;
        }

        private void OnDestroy()
        {
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }

        public void OnPointEnd()
        {
            UnityEngine.GameObject.Destroy(this);
        }
        public void OnPointStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }
        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }
    }
}