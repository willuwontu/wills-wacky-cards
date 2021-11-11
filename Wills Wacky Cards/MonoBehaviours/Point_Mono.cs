using UnityEngine;
using WillsWackyCards.Extensions;
using UnboundLib.GameModes;
using UnboundLib;
using System.Collections.Generic;
using System;

namespace WillsWackyCards.MonoBehaviours
{
    public class AdrenalineRush_Mono : Point_Mono
    {

    }

    public class EnduranceTraining_Mono : Point_Mono
    {

    }

    public class Point_Mono : Hooked_Mono
    {
        public Func<float, int, float, bool, float> MultiplierCalculation = 
            (start, points, multiplier, up) => 
            {
                var result = 1f;
                result = Mathf.Max(start + points * multiplier, 0.0001f);
                if (!up)
                {
                    result = 1/result;
                }
                //UnityEngine.Debug.Log($"Stat multiplier is {result}");
                return result; 
            };
        public float multiplierPerPoint = 1f;
        public float startValue = 1f;

        private List<TeamScore> currentScore = new List<TeamScore>();
        private int totalPointsEarned = 0;

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

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void ChangeStats(bool apply = true)
        {
            var upMult = MultiplierCalculation(startValue, totalPointsEarned, multiplierPerPoint, true);
            var downMult = MultiplierCalculation(startValue, totalPointsEarned, multiplierPerPoint, false);

            if (!apply)
            {
                upMult = 1f/upMult;
                downMult = 1f/downMult;
            }

            data.maxHealth *= upMult;
            data.health *= upMult;
            stats.InvokeMethod("ConfigureMassAndSize");
            stats.movementSpeed *= upMult;
            stats.jump *= upMult;
            gravity.gravityForce *= downMult;
            gun.damage *= upMult;
            gunAmmo.reloadTimeMultiplier *= downMult;
            block.cdMultiplier *= downMult;
        }

        private void UpdateMultiplier()
        {
            currentScore.Clear();
            foreach (var player in PlayerManager.instance.players)
            {
                currentScore.Add(GameModeManager.CurrentHandler.GetTeamScore(player.teamID));

                //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Debugging] Team {player.teamID} has {GameModeManager.CurrentHandler.GetTeamScore(player.teamID).points} points.");
            }

            totalPointsEarned = 0;

            foreach (var score in currentScore)
            {
                totalPointsEarned += score.points;
            }

            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Debugging] Current number of points is {totalPointsEarned}");
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