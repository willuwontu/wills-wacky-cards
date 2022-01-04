using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.GameModes;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WWC.MonoBehaviours
{
    public class PointCard_Mono : Hooked_Mono
    {
        private TextMeshProUGUI description;
        private TextMeshProUGUI cardName;

        private StatHolder[] stats = new StatHolder[] { };

        private List<TeamScore> currentScore = new List<TeamScore>();
        private int totalPointsEarned = 0;

        public Func<float, int, float, bool, float> MultiplierCalculation =
            (start, points, multiplier, up) =>
            {
                var result = 1f;
                result = Mathf.Max(start + points * multiplier, 0.0001f);
                if (!up)
                {
                    result = 1 / result;
                }
                return result;
            };
        public float multiplierPerPoint = 1f;
        public float startValue = 1f;

        private class StatHolder
        {
            public TextMeshProUGUI stat;
            public TextMeshProUGUI value;
        }

        private void Start()
        {
            HookedMonoManager.instance.hookedMonos.Add(this);


            TextMeshProUGUI[] allChildrenRecursive = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            GameObject effectText = allChildrenRecursive.Where(obj => obj.gameObject.name == "EffectText").FirstOrDefault().gameObject;
            GameObject titleText = allChildrenRecursive.Where(obj => obj.gameObject.name == "Text_Name").FirstOrDefault().gameObject;

            LayoutElement[] statHolders = gameObject.GetComponentsInChildren<LayoutElement>().Where(obj => obj.gameObject.name == "StatObject(Clone)").ToArray();
            List<StatHolder> temp = new List<StatHolder>();

            foreach (var item in statHolders)
            {
                StatHolder stat = new StatHolder();
                stat.stat = item.gameObject.GetComponentsInChildren<TextMeshProUGUI>().Where(obj => obj.gameObject.name == "Stat").FirstOrDefault();
                stat.value = item.gameObject.GetComponentsInChildren<TextMeshProUGUI>().Where(obj => obj.gameObject.name == "Value").FirstOrDefault();

                temp.Add(stat);
            }

            stats = temp.ToArray();

            description = effectText.GetComponent<TextMeshProUGUI>();
            cardName = titleText.GetComponent<TextMeshProUGUI>();
        }

        public override void OnPointStart()
        {
            UpdateMultiplier();
            foreach (var statInfo in CurrentStats())
            {
                var stat = stats.Where((stat) => { /*UnityEngine.Debug.Log($"Comparing {stat.stat.text} to {statInfo.stat}");*/ return stat.stat.text == statInfo.stat; }).FirstOrDefault();
                stat.value.text = statInfo.amount;

                if (statInfo.positive)
                {
                    stat.value.color = new Color(0.4654f, 0.6038f, 0.3902f);
                }
                else
                {
                    stat.value.color = new Color(0.6981f, 0.326f, 0.326f);
                }
            }
        }

        public override void OnGameStart()
        {
            OnPointStart();
        }

        public override void OnPickStart()
        {
            OnPointStart();
        }

        private CardInfoStat[] CurrentStats()
        {
            var upMult = MultiplierCalculation(startValue, totalPointsEarned, multiplierPerPoint, true);
            var downMult = MultiplierCalculation(startValue, totalPointsEarned, multiplierPerPoint, false);
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = upMult >=1f,
                    stat = "HP",
                    amount = string.Format("{0}{1:F0}%", upMult >=1f ? "+" : "", (upMult-1f)*100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = upMult >=1f,
                    stat = "Movespeed",
                    amount = string.Format("{0}{1:F0}%", upMult >=1f ? "+" : "", (upMult-1f)*100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = upMult >=1f,
                    stat = "Damage",
                    amount = string.Format("{0}{1:F0}%", upMult >=1f ? "+" : "", (upMult-1f)*100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = downMult <= 1f,
                    stat = "Reload Time",
                    amount = string.Format("{0}{1:F0}%", downMult <= 1f ? "-" : "+", Mathf.Abs((1f/downMult)-1f)*100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = downMult <= 1f,
                    stat = "Block Cooldown",
                    amount = string.Format("{0}{1:F0}%", downMult <= 1f ? "-" : "+", Mathf.Abs((1f/downMult)-1f)*100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }

        private void UpdateMultiplier()
        {
            currentScore.Clear();
            foreach (var player in PlayerManager.instance.players)
            {
                currentScore.Add(GameModeManager.CurrentHandler.GetTeamScore(player.teamID));

                WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Debugging] Team {player.teamID} has {GameModeManager.CurrentHandler.GetTeamScore(player.teamID).points} points.");
            }

            totalPointsEarned = 0;

            foreach (var score in currentScore)
            {
                totalPointsEarned += score.points;
            }

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Debugging] Current number of points is {totalPointsEarned}");
        }
    }
}