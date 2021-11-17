using System;
using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using WillsWackyManagers.Utils;
using WWC.MonoBehaviours;
using UnboundLib.Cards;
using UnityEngine;

namespace WWC.Cards
{
    class RunicWards : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.allowMultiple = false;
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(20, () => CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); }));
            player.gameObject.GetOrAddComponent<RunicWardsBlock_Mono>();
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Runic Wards";
        }
        protected override string GetDescription()
        {
            return "The first person to hurt you each round gains a curse.";
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
                    stat = "Block per Curse",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Curse",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
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

    class RunicWardsBlock_Mono : Hooked_Mono
    {
        public int shields = 0;
        public bool damaged = false;
        public Player attacker = null;
        public int additionalBlocks = 0;
        private bool increased;

        private CharacterData data;
        private Player player;
        private Block block;

        private void Start()
        {
            HookedMonoManager.instance.hookedMonos.Add(this);
            data = GetComponentInParent<CharacterData>();
        }

        private void Update()
        {
            if (!player)
            {
                if (!(data is null))
                {
                    player = data.player;
                    block = data.block;
                }

            }
        }

        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName == "Runic Wards")
                {
                    haveCard = true;
                    break;
                }
            }

            if (!haveCard)
            {
                UnityEngine.GameObject.Destroy(this);
            }
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnBattleStart()
        {
            damaged = false;
            shields = CurseManager.instance.GetAllCursesOnPlayer(player).Count();
        }

        public override void OnPointStart()
        {
            CheckIfValid();
            var curses = CurseManager.instance.GetAllCursesOnPlayer(player);
            
            if (!increased)
            {
                increased = true;
                additionalBlocks = curses.Count();
                block.additionalBlocks -= additionalBlocks;
            }
        }

        public override void OnPointEnd()
        {
            if (increased)
            {
                increased = false;
                block.additionalBlocks -= additionalBlocks;
            }

            if (damaged && attacker != null)
            {
                CurseManager.instance.CursePlayer(attacker, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); });
                attacker = null;
                damaged = false;
            }
        }

        private void OnDestroy()
        {
            if (increased)
            {
                increased = false;
                block.additionalBlocks -= additionalBlocks;
            }
        }
    }

    [HarmonyPatch(typeof(ProjectileHit), "RPCA_DoHit")]
    [HarmonyPriority(Priority.First)]
    class ProjectileHitPatchRPCA_DoHit
    {
        private static void Prefix(ProjectileHit __instance, Vector2 hitPoint, Vector2 hitNormal, Vector2 vel, int viewID, int colliderID, ref bool wasBlocked)
        {
            // prefix to allow autoblocking

            HitInfo hitInfo = new HitInfo();
            hitInfo.point = hitPoint;
            hitInfo.normal = hitNormal;
            hitInfo.collider = null;
            if (viewID != -1)
            {
                PhotonView photonView = PhotonNetwork.GetPhotonView(viewID);
                hitInfo.collider = photonView.GetComponentInChildren<Collider2D>();
                hitInfo.transform = photonView.transform;
            }
            else if (colliderID != -1)
            {
                hitInfo.collider = MapManager.instance.currentMap.Map.GetComponentsInChildren<Collider2D>()[colliderID];
                hitInfo.transform = hitInfo.collider.transform;
            }
            HealthHandler healthHandler = null;
            if (hitInfo.transform)
            {
                healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
            }
            if (healthHandler && healthHandler.GetComponent<CharacterData>() && healthHandler.GetComponent<Block>())
            {
                var warded = healthHandler.GetComponent<RunicWardsBlock_Mono>();
                if (!warded)
                {
                    return;
                }
                Block block = healthHandler.GetComponent<Block>();
                if (warded.shields > 0 && !block.IsBlocking())
                {
                    wasBlocked = true;
                    warded.shields--;
                    if (healthHandler.GetComponent<CharacterData>().view.IsMine)
                    { 
                        block.CallDoBlock(true, true, BlockTrigger.BlockTriggerType.Default, default(Vector3), false); 
                    }
                }
            }
        }
    }
}
