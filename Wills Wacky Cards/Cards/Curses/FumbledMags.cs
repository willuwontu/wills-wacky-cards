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
using WillsWackyManagers.Utils;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using Photon.Pun;
using WillsWackyManagers.UnityTools;

namespace WWC.Cards.Curses
{
    class FumbledMags : CustomCard, ICurseCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseCategory };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<FumbledMags_Mono>();
            mono.chance += 5;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<FumbledMags_Mono>();
            mono.chance -= 5;
            if (mono.chance <= 0)
            {
                UnityEngine.GameObject.Destroy(mono);
            }
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Curse] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Fumbled Mags";
        }
        protected override string GetDescription()
        {
            return "Whoops, slipped right out of your fingers.";
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
                    stat = "Reload Fumbles",
                    amount = "+5%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
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
    [DisallowMultipleComponent]
    public class FumbledMags_Mono : MonoBehaviourPun, IBattleStartHookHandler, IPointEndHookHandler, IGameStartHookHandler
    {
        public int chance = 0;

        private bool canTrigger = false;

        private CharacterData data;
        private Player player;
        private Gun gun;
        private GunAmmo gunAmmo;
        private CharacterStatModifiers stats;

        private void Start()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            data = GetComponentInParent<CharacterData>();
            player = data.player;
            stats = data.stats;
            gun = player.data.weaponHandler.gun;
            gunAmmo = gun.GetComponentInChildren<GunAmmo>();
            stats.OnReloadDoneAction += OnReload;
        }

        private void OnReload(int loadedAmmo)
        {
            if (canTrigger)
            {
                if (photonView.IsMine && UnityEngine.Random.Range(0,100) < chance)
                {
                    photonView.RPC(nameof(RemoveAmmo), RpcTarget.All, loadedAmmo);
                }
            }
        }

        [PunRPC]
        private void RemoveAmmo(int ammo)
        {
            gunAmmo.SetFieldValue("currentAmmo", (int) gunAmmo.GetFieldValue("currentAmmo") - ammo);
        }

        public void OnBattleStart()
        {
            canTrigger = true;
        }

        public void OnPointEnd()
        {
            canTrigger = false;
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {

            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}