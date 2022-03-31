using UnityEngine;
using WWC.Extensions;
using WWC.Interfaces;
using UnboundLib.GameModes;
using UnboundLib;
using System.Collections.Generic;
using System;
using System.Linq;

namespace WWC.MonoBehaviours
{
    public class AdrenalineRush_Mono : Point_Mono
    {

    }

    public class EnduranceTraining_Mono : Point_Mono
    {

    }

    public class Point_Mono : ReversibleEffect, IPointStartHookHandler, IPointEndHookHandler, IGameStartHookHandler
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

        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            applyImmediately = false;
            this.SetLivesToEffect(int.MaxValue);
        }

        public void OnPointStart()
        {
            UpdateMultipliers();
            this.ApplyModifiers();
        }

        public void OnPointEnd()
        {
            this.ClearModifiers();
        }

        public void OnGameStart()
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

        private void UpdateMultipliers()
        {
            currentScore.Clear();

            currentScore = PlayerManager.instance.players.Select(p => p.teamID).Distinct().Select(ID => GameModeManager.CurrentHandler.GetTeamScore(ID)).ToList();

            totalPointsEarned = 0;

            foreach (var score in currentScore)
            {
                totalPointsEarned += score.points;
            }

            var upMult = MultiplierCalculation(startValue, totalPointsEarned, multiplierPerPoint, true);
            var downMult = MultiplierCalculation(startValue, totalPointsEarned, multiplierPerPoint, false);

            this.characterDataModifier.maxHealth_mult = upMult;
            this.characterDataModifier.health_mult = upMult;
            this.characterStatModifiersModifier.movementSpeed_mult = upMult;
            this.characterStatModifiersModifier.jump_mult = upMult;
            this.gravityModifier.gravityForce_mult = downMult;
            this.gunStatModifier.damage_mult = upMult;
            this.gunAmmoStatModifier.reloadTimeMultiplier_mult = downMult;
            this.blockModifier.cdMultiplier_mult = downMult;

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Debugging] Current number of points is {totalPointsEarned}");
        }

        public override void OnOnDestroy()
        {
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }
    }
}