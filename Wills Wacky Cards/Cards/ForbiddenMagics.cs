using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnboundLib.Utils;
using WillsWackyCards.Extensions;
using WillsWackyManagers.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ModdingUtils.Extensions;
using UnityEngine;

namespace WillsWackyCards.Cards
{
    class ForbiddenMagics : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.GetAdditionalData().canBeReassigned = false;
            // Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[WWC][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            if (CurseManager.instance.GetRaw().Count() > 0)
            {
                CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); });

                foreach (var item in PlayerManager.instance.players.Where(other => other.teamID != player.teamID).ToList())
                {
                    CurseManager.instance.CursePlayer(item, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(item, curse); });
                    CurseManager.instance.CursePlayer(item, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(item, curse); });
                    CurseManager.instance.CursePlayer(item, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(item, curse); });
                }
            }
            UnityEngine.Debug.Log($"[WWC][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Drives me crazy
            UnityEngine.Debug.Log($"[WWC][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Forbidden Magics";
        }
        protected override string GetDescription()
        {
            return "Burn yourself to hurt your foes.";
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
                    positive = false,
                    stat = "Curse",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Curses to Foes",
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
            return "WWC";
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }
}
