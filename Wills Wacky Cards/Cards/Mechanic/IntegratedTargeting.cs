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
using UnityEngine;
using ClassesManagerReborn.Util;

namespace WWC.Cards
{
    class IntegratedTargeting : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.gravity = 0f;
            gun.spread = 0f;
            cardInfo.allowMultiple = false;

            gameObject.GetOrAddComponent<ClassNameMono>().className = MechanicClass.name;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var upgrader = player.GetComponentInChildren<MechanicUpgrader>();

            if (upgrader)
            {
                upgrader.gunStatModifier.percentageDamage_add += 0.05f;
                upgrader.upgradeAction += new Action<int>(level =>
                {
                    if (level == 5)
                    {
                        player.gameObject.AddComponent<TargetingSensors_Mono>();
                    }
                });
                upgrader.upgradeTimeAdd += 2f;
            }

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        internal static CardInfo card = null;
        protected override string GetTitle()
        {
            return "Integrated Targeting Sensors";
        }
        protected override string GetDescription()
        {
            return $"<color=#FF0000>ANALYZING WEAKPOINTS ...</color>";
        }
        protected override GameObject GetCardArt()
        {
            GameObject art;

            try
            {
                art = WillsWackyManagers.WillsWackyManagers.instance.WWWMAssets.LoadAsset<GameObject>("C_IntegratedTargeting");
            }
            catch
            {
                art = null;
            }

            return art;
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
                    stat = "%Max HP damage per upgrade",
                    amount = "+5%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "At 5 Upgrades",
                    amount = "Unblockable Bullets",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Shots",
                    amount = "Focused",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet Gravity",
                    amount = "No",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Upgrade Time",
                    amount = "+2s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.TechWhite;
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
    public class TargetingSensors_Mono : MonoBehaviour, IPointStartHookHandler
    {
        private void Start()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            this.gameObject.GetComponent<CharacterData>().weaponHandler.gun.unblockable = true;
        }
        private void OnDestroy()
        {
            var cards = this.gameObject.GetComponent<CharacterData>().currentCards.ToArray();

            if (!(cards.Where(card => card.gameObject.GetComponent<Gun>().unblockable).Count() > 0))
            {
                this.gameObject.GetComponent<CharacterData>().weaponHandler.gun.unblockable = false;
            }
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }
        public void OnPointStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }
    }
}