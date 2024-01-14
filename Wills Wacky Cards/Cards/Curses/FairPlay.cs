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
using ModdingUtils.Extensions;

namespace WWC.Cards.Curses
{
    class FairPlay : CustomCard, ICurseCard, IConditionalCard
    {
        private static CardInfo card;
        public CardInfo Card { get => card; set { if (!card) { card = value; } } }
        public bool Condition(Player player, CardInfo card)
        {
            if (card != FairPlay.card)
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

            if (player.data.block.additionalBlocks > 10)
            {
                return true;
            }

            if (player.data.block.BlocksPerSecond() > 3f)
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
            player.gameObject.AddComponent<FairPlayMono>();
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<FairPlayMono>();
            UnityEngine.GameObject.Destroy(mono);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Fair Play";
        }
        protected override string GetDescription()
        {
            return "C'mon you gotta play fair.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return Rarities.Scarce;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {

            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CurseManager.instance.FracturedBlue;
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
    public class FairPlayMono : MonoBehaviour
    {
        Player player;
        float lastVulnerable = 0f;

        private void Awake()
        {
            this.player = GetComponentInParent<Player>();
        }

        private void Update()
        {
            if (ModdingUtils.Utils.PlayerStatus.PlayerAliveAndSimulated(player))
            {
                if (player.data.block.IsBlocking())
                {
                    lastVulnerable += Time.deltaTime;
                }
                else
                {
                    lastVulnerable = 0f;
                }
            }
            else
            {
                lastVulnerable = 0f;
            }

            if (lastVulnerable > 5f)
            {
                player.data.view.RPC("RPCA_Die", Photon.Pun.RpcTarget.All, new object[] { Vector2.up });
            }
        }
    }
}