using System;
using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using WWC.Extensions;
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
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseSpawner };
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(20, () => CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); }));
            var blockMono = player.gameObject.GetOrAddComponent<RunicWardsBlock_Mono>();
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
            return "For every 200 points of damage a person deals to you, they get a curse.";
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
                    positive = true,
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

    [DisallowMultipleComponent]
    class RunicWardsBlock_Mono : Hooked_Mono
    {
        public int shields = 0;
        public int additionalBlocks = 0;
        private bool increased;
        internal Dictionary<Player, float> damageTracker = new Dictionary<Player, float>();

        private CharacterData data;
        private Player player;
        private Block block;
        private CharacterStatModifiers stats;

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
                    stats = data.stats;
                    stats.WasDealtDamageAction += OnWasDealtDamage;
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

        private void OnWasDealtDamage(Vector2 damage, bool selfDamage)
        {
            var target = data.lastSourceOfDamage;

            if (ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(target.data).isAIMinion)
            {
                target = ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(target.data).spawner;
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Runic Wards][Debugging] Player {data.lastSourceOfDamage.playerID} was an AI with player {target.playerID} as it's master.");
            }

            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Runic Wards][Debugging] Player {player.playerID} was damaged for {damage.magnitude} by player {target.playerID}");

            if (damageTracker.Keys.ToArray().Contains(target))
            {
                damageTracker[target] += damage.magnitude;
            }
            else
            {
                damageTracker.Add(target, damage.magnitude);
            }
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnBattleStart()
        {
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
            if (PhotonNetwork.OfflineMode || this.photonView.IsMine)
            {
                var blah = damageTracker.ToDictionary((kvp) => kvp.Key, (kvp) => kvp.Value);
                foreach (var trackedDamage in blah)
                {
                    if (PhotonNetwork.OfflineMode)
                    {
                        RPCA_GiveCurse(trackedDamage.Key.playerID, trackedDamage.Value);
                    }
                    else
                    {
                        this.photonView.RPC(nameof(RPCA_GiveCurse), RpcTarget.All, trackedDamage.Key.playerID, trackedDamage.Value);
                    }
                }
            }
        }

        [PunRPC]
        private void RPCA_GiveCurse(int id, float amount)
        {
            var person = PlayerManager.instance.GetPlayerWithID(id);
            var counter = amount;

            while (counter >= 200f)
            {
                CurseManager.instance.CursePlayer(person, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(person, curse); });
                counter -= 200f;
            }

            damageTracker[person] = counter;
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Runic Wards][Debugging] Player {player.playerID} has a total of {counter} damage from player {person.playerID} after giving out curses.");
        }

        private void OnDestroy()
        {
            if (increased)
            {
                increased = false;
                block.additionalBlocks -= additionalBlocks;
            }
            stats.WasDealtDamageAction -= OnWasDealtDamage;
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
