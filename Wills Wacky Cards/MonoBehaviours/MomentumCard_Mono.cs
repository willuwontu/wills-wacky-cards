using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WWC.MonoBehaviours
{
    public class MomentumCard_Mono : MonoBehaviour
    {
        private TextMeshProUGUI description;
        private TextMeshProUGUI cardName;
        public CardInfo card;
        public bool updated = false;
        public bool alwaysUpdate = false;
        public bool builtCard = false;
        public int stacks;
        public string title;
        public Func<CardInfoStat[]> statsGenerator;

        private StatHolder[] stats = new StatHolder[] { };

        private class StatHolder
        {
            public TextMeshProUGUI stat;
            public TextMeshProUGUI value;
        }

        private void Awake()
        {

        }
        private void Start()
        {
            TextMeshProUGUI[] allChildrenRecursive = this.gameObject.GetComponentsInChildren<TextMeshProUGUI>();

            if (allChildrenRecursive.Length < 1)
            {
                return;
            }

            GameObject effectText = allChildrenRecursive.Where(obj => obj.gameObject.name == "EffectText").FirstOrDefault().gameObject;
            GameObject titleText = allChildrenRecursive.Where(obj => obj.gameObject.name == "Text_Name").FirstOrDefault().gameObject;

            LayoutElement[] statHolders = this.gameObject.GetComponentsInChildren<LayoutElement>().Where(obj => obj.gameObject.name == "StatObject(Clone)").ToArray();
            List<StatHolder> temp = new List<StatHolder>();

            foreach (var item in statHolders)
            {
                StatHolder stat = new StatHolder();
                stat.stat = item.gameObject.GetComponentsInChildren<TextMeshProUGUI>().Where(obj => obj.gameObject.name == "Stat").FirstOrDefault();
                stat.value = item.gameObject.GetComponentsInChildren<TextMeshProUGUI>().Where(obj => obj.gameObject.name == "Value").FirstOrDefault();

                temp.Add(stat);
            }

            stats = temp.ToArray();

            this.description = effectText.GetComponent<TextMeshProUGUI>();
            this.cardName = titleText.GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
            if (!this.cardName)
            {
                return;
            }

            if (updated || (alwaysUpdate && (this.stacks != MomentumTracker.stacks)))
            {
                if (alwaysUpdate)
                {
                    this.stacks = MomentumTracker.stacks;
                }

                updated = false;

                cardName.text = title.ToUpper();
                foreach (var statInfo in (alwaysUpdate ? statsGenerator() : card.cardStats))
                {
                    var stat = stats.Where((stat) => stat.stat.text == statInfo.stat).FirstOrDefault();
                    stat.value.text = statInfo.amount;
                    //foreach (var stat in stats)
                    //{
                    //    //UnityEngine.Debug.Log($"Comparing {statInfo.stat} and {stat.stat.text}");
                    //    if (statInfo.stat == stat.stat.text)
                    //    {
                    //        stat.value.text = statInfo.amount;
                    //        break;
                    //    }
                    //}
                }
            }
        }
    }
    public class MomentumTracker
    {
        public static int stacks = 0;
        public static Dictionary<int, CardInfo> createdOffenseCards = new Dictionary<int, CardInfo>();
        public static Dictionary<int, CardInfo> createdDefenseCards = new Dictionary<int, CardInfo>();

        public static void AddOfffenseCard(CardInfo cardInfo, Player player)
        {
            createdOffenseCards.Add(MomentumTracker.stacks, cardInfo);
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo);
            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, cardInfo, false, "", 2f, 2f, true);

        }
        public static void AddDeffenseCard(CardInfo cardInfo, Player player)
        {
            createdOffenseCards.Add(MomentumTracker.stacks, cardInfo);
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo);
            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, cardInfo, false, "", 2f, 2f, true);

        }

        public static CardInfoStat[] GetOffensiveMomentumStats(int stacks)
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Ammo",
                    amount = $"+{stacks}",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Attack Speed",
                    amount = string.Format("+{0:F0}%",(1f - (float) Math.Pow(.95f, stacks)) * 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet Speed",
                    amount = string.Format("+{0:F0}%",((float) Math.Pow(1.05f, stacks) -1f) * 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Damage",
                    amount = string.Format("+{0:F0}%",((float) Math.Pow(1.05f, stacks) -1f) * 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bounces",
                    amount = $"+{stacks/2}",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bursts",
                    amount = $"+{stacks/10}",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullets",
                    amount = $"+{stacks/5}",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        public static CardInfoStat[] GetDefensiveMomentumStats(int stacks)
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Regeneration",
                    amount = $"+{stacks} hp/s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Gravity",
                    amount = string.Format("-{0:F0}%",(1f - (float) Math.Pow(.95f, stacks)) * 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Health",
                    amount = string.Format("+{0:F0}%",((float) Math.Pow(1.05f, stacks) -1f) * 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Jump Height",
                    amount = string.Format("+{0:F0}%",((float) Math.Pow(1.05f, stacks) -1f) * 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Jumps",
                    amount = $"+{stacks/3}",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Additional Blocks",
                    amount = $"+{stacks/7}",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Lifesteal",
                    amount = string.Format("+{0:F0}%",((float) Math.Pow(1.05f, stacks) -1f) * 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
    }
}
