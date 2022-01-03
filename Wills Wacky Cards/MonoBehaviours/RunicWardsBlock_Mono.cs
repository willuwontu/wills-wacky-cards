using System;
using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using WWC.Extensions;
using WillsWackyManagers.Utils;
using UnityEngine;
using WWC.MonoBehaviours;

namespace WWC.MonoBehaviours
{
    [DisallowMultipleComponent]
    class RunicWardsBlock_Mono : Hooked_Mono
    {
        public int shields = 0;
        public int additionalBlocks = 0;
        private bool increased;
        internal Dictionary<Player, float> damageTracker = new Dictionary<Player, float>();
        private float damageNeeded = 600f;

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
            try
            {
                RecordDealtDamage(damage);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        private void RecordDealtDamage(Vector2 damage)
        {
            var target = data.lastSourceOfDamage;

            if (!target)
            {
                return;
            }

            if (target == null)
            {
                return;
            }

            if (ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(target.data).isAIMinion)
            {
                target = ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(target.data).spawner;
                //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Runic Wards][Debugging] Player {data.lastSourceOfDamage.playerID} was an AI with player {target.playerID} as it's master.");
            }

            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Runic Wards][Debugging] Player {player.playerID} was damaged for {damage.magnitude} by player {target.playerID}");

            if (damageTracker.Keys.ToArray().Contains(target))
            {
                damageTracker[target] += Mathf.Clamp(damage.magnitude, 0f, data.maxHealth);
            }
            else
            {
                damageTracker.Add(target, Mathf.Clamp(damage.magnitude, 0f, data.maxHealth));
            }
        }

        public override void OnGameStart()
        {
            Destroy(this);
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
                block.additionalBlocks += additionalBlocks;
            }
        }

        public override void OnPointEnd()
        {
            if (increased)
            {
                increased = false;
                block.additionalBlocks -= additionalBlocks;
            }

            if (PhotonNetwork.OfflineMode || photonView.IsMine)
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
                        photonView.RPC(nameof(RPCA_GiveCurse), RpcTarget.All, trackedDamage.Key.playerID, trackedDamage.Value);
                    }
                }
            }
        }

        [PunRPC]
        private void RPCA_GiveCurse(int id, float amount)
        {
            var person = PlayerManager.instance.GetPlayerWithID(id);
            var counter = amount;

            while (counter >= damageNeeded)
            {
                CurseManager.instance.CursePlayer(person, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(person, curse); });
                counter -= damageNeeded;
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
}

namespace WWC.Patches
{
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
                        block.CallDoBlock(true, true, BlockTrigger.BlockTriggerType.Default, default, false);
                    }
                }
            }
        }
    }
}
