using UnityEngine;
using WillsWackyCards.Extensions;
using UnboundLib.GameModes;
using UnboundLib;
using System.Collections.Generic;

namespace WillsWackyCards.MonoBehaviours
{
    public class Marathon_Mono : Hooked_Mono
    {
        private Dictionary<int, TeamScore> currentScore = new Dictionary<int, TeamScore>();
        private int totalPointsEarned = 0;
        public float multiplierPerPoint = 1f;
        private bool coroutineStarted;
        private CharacterData data;
        private Player player;
        private Block block;
        private HealthHandler health;
        private CharacterStatModifiers stats;
        private Gun gun;
        private GunAmmo gunAmmo;
        private Gravity gravity;
        private WeaponHandler weaponHandler;

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
                    weaponHandler = data.weaponHandler;
                    gun = weaponHandler.gun;
                    gunAmmo = gun.GetComponentInChildren<GunAmmo>();
                    block = data.block;
                    stats = data.stats;
                    health = data.healthHandler;
                    gravity = player.GetComponent<Gravity>();
                }

            }
        }

        public override void OnPointStart()
        {
            UpdateMultiplier();
            ChangeStats();
        }

        public override void OnPointEnd()
        {
            ChangeStats(false);
        }

        private void ChangeStats(bool apply = true)
        {
            var upMult = Mathf.Pow(1f + multiplierPerPoint, (float) totalPointsEarned);
            var downMult = Mathf.Pow(Mathf.Max(1f - multiplierPerPoint, 0.0001f), (float) totalPointsEarned);

            if (apply)
            {
                upMult = 1f/upMult;
                downMult = 1f/downMult;
            }

            data.maxHealth *= upMult;
            stats.InvokeMethod("ConfigureMassAndSize");
            stats.movementSpeed *= upMult;
            gravity.gravityForce *= downMult;
            gun.damage *= upMult;
            gun.attackSpeed *= downMult;
            gun.projectileSpeed *= upMult;
            block.cdMultiplier *= downMult;
        }

        public CardInfoStat[] CurrentStats()
        {
            var upMult = Mathf.Pow(1f + multiplierPerPoint, (float) totalPointsEarned);
            var downMult = 1 / upMult;
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Health",
                    amount = string.Format("{0}{1:F1}%", (upMult >=1f ? "+" : ""), upMult-1f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Movespeed",
                    amount = string.Format("{0}{1:F1}%", (upMult >=1f ? "+" : ""), upMult-1f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Damage",
                    amount = string.Format("{0}{1:F1}%", (upMult >=1f ? "+" : ""), upMult-1f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Projectile Speed",
                    amount = string.Format("{0}{1:F1}%", (upMult >=1f ? "+" : ""), upMult-1f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Attack Speed",
                    amount = string.Format("{0}{1:F1}%", (downMult <= 1f ? "+" : ""), 1f - downMult),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Block Cooldown",
                    amount = string.Format("{0}{1:F1}%", (downMult <= 1f ? "-" : "+"), Mathf.Abs(1f - downMult)),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }

        internal void UpdateMultiplier()
        {
            foreach (var player in PlayerManager.instance.players)
            {
                if (currentScore.TryGetValue(player.teamID, out var score))
                {
                    score = GameModeManager.CurrentHandler.GetTeamScore(player.teamID);
                }
                else
                {
                    currentScore.Add(player.teamID, GameModeManager.CurrentHandler.GetTeamScore(player.teamID));
                }
            }

            totalPointsEarned = 0;

            foreach (var score in currentScore.Values)
            {
                totalPointsEarned += score.points;
            }
        }

        private void OnDestroy()
        {
            HookedMonoManager.instance.hookedMonos.Remove(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}