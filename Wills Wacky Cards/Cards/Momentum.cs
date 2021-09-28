using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WillsWackyCards.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WillsWackyCards.Cards
{
    class Momentum : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            var tracker = this.gameObject.GetOrAddComponent<MomentumTracker_Mono>();
            tracker.card = cardInfo;
            MomentumTracker.stacks += 1;
            var stacks = MomentumTracker.stacks = Math.Max(MomentumTracker.stacks, 1);

            UnityEngine.Debug.Log($"[WWC][Card] {stacks} Momentum Stacks built up");
            gun.ammo = stacks;
            gun.attackSpeed = (float) Math.Pow(.95f, (double) stacks);
            gun.projectileSpeed = (float) Math.Pow(1.05f, stacks);
            gun.reflects = stacks / 2;
            gun.damage = (float) Math.Pow(1.05f, stacks);
            gun.bursts = stacks / 10;
            gun.numberOfProjectiles = stacks / 5;

            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("CardManipulation") };

            cardInfo.cardStats = MomentumTracker.GetMomentumStats(stacks);
            tracker.updated = true;

            UnityEngine.Debug.Log("[WWC][Card] Momentum Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[WWC] Player {player.teamID} has {data.currentCards.Count} Cards");
            var mono = player.gameObject.GetOrAddComponent<Momentum_Mono>();
            mono.cards.Add(new Momentum_Mono.CardStack { cardIndex = player.data.currentCards.Count, stacks = MomentumTracker.stacks });
            UnityEngine.Debug.Log($"[WWC] Player {player.teamID} has {mono.cards[mono.cards.Count-1].cardIndex} Cards");

            gun.timeBetweenBullets = 0.1f;
            gun.spread = Mathf.Clamp(gun.spread, 0.1f, 1);
            if (MomentumTracker.playerStacks.TryGetValue(player, out var stacks))
            {
                stacks += MomentumTracker.stacks;
            }
            else
            {
                MomentumTracker.playerStacks.Add(player, MomentumTracker.stacks);
            }

            MomentumTracker.stacks = 0;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Drives me crazy
        }

        protected override string GetTitle()
        {
            return "Momentum";
        }
        protected override string GetDescription()
        {
            return "Stats increase each time it appears without being taken.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return MomentumTracker.GetMomentumStats(MomentumTracker.stacks);
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
        }
        public override string GetModName()
        {
            return "WWC";
        }
        public override bool GetEnabled()
        {
            return true;
        }
        private class MomentumTracker_Mono : MonoBehaviour
        {
            private TextMeshProUGUI description;
            private TextMeshProUGUI cardName;
            public CardInfo card;
            public bool updated = false;

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
                if (updated)
                {
                    foreach (var statInfo in card.cardStats)
                    {
                        foreach (var stat in stats)
                        {
                            //UnityEngine.Debug.Log($"Comparing {statInfo.stat} and {stat.stat.text}");
                            if (statInfo.stat == stat.stat.text)
                            {
                                stat.value.text = statInfo.amount;
                                break;
                            }
                        }
                    }
                    updated = false;
                }
            }
        }
    }

    public class Momentum_Mono : MonoBehaviour
    {
        public Player player;
        public List<CardStack> cards = new List<CardStack>();

        public class CardStack
        {
            public int cardIndex;
            public int stacks;
        }

        private void UpdateIndecesOnRemove(Player player, CardInfo card, int index)
        {

        }

        private void Start()
        {


            //ModdingUtils.Utils.Cards.instance.AddOnRemoveCallback(UpdateIndecesOnRemove);
        }
    }

    public class MomentumTracker
    {
        public static int stacks = 0;
        public static Dictionary<Player, int> playerStacks = new Dictionary<Player, int>();
        public static Dictionary<Player, bool> trackRemoval = new Dictionary<Player, bool>();

        public static CardInfoStat[] GetMomentumStats(int stacks)
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
    }
}
