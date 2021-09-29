using System;
using System.Collections;
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
    class UnstoppableForce : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            var tracker = this.gameObject.GetOrAddComponent<MomentumCard_Mono>();
            tracker.card = cardInfo;
            tracker.title = "Unstoppable Force";

            MomentumTracker.stacks += 1;

            if (!MomentumTracker.createdOffenseCards.TryGetValue(MomentumTracker.stacks, out var cardData))
            {
                CustomCard.BuildCard<BuildUnstoppableForce>(cardInfo => { MomentumTracker.createdOffenseCards.Add(MomentumTracker.stacks, cardInfo); ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo); });
            }
            var stacks = MomentumTracker.stacks = Math.Max(MomentumTracker.stacks, 1);

            UnityEngine.Debug.Log($"[WWC][Card] {stacks} Momentum Stacks built up");

            cardInfo.cardStats = MomentumTracker.GetOffensiveMomentumStats(stacks);
            tracker.updated = true;

            UnityEngine.Debug.Log("[WWC][Card] Unstoppable Force Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var cleaner = player.gameObject.GetOrAddComponent<MomentumCleanup_Mono>();

            if (MomentumTracker.createdOffenseCards.TryGetValue(MomentumTracker.stacks, out var cardInfo))
            {
                MomentumTracker.building = false;
                UnityEngine.Debug.Log($"[WWC][Momentum]{cardInfo.cardName} found.");
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, cardInfo, false, "", 2f, 2f, true);
            }
            else
            {
                UnityEngine.Debug.Log($"[WWC][Momentum] No Card Found, creating new one.");
                CustomCard.BuildCard<BuildUnstoppableForce>(cardInfo => { MomentumTracker.AddOffenseCard(cardInfo, player); });
            }

            cleaner.CleanUp();
        }
                
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Drives me crazy
        }

        protected override string GetTitle()
        {
            return "Unstoppable Force";
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
            return MomentumTracker.GetOffensiveMomentumStats(MomentumTracker.stacks);
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }
        public override string GetModName()
        {
            return "WWC";
        }
        public override bool GetEnabled()
        {
            return true;
        }
    }

    class BuildUnstoppableForce : UnstoppableForce
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            var tracker = this.gameObject.GetOrAddComponent<MomentumCard_Mono>();
            int stacks;
            if (!tracker.builtCard)
            {
                tracker.card = cardInfo;
                tracker.title = "Unstoppable Force";
                tracker.stacks = Math.Max(MomentumTracker.stacks, 1);
                tracker.builtCard = true;
            }
            stacks = tracker.stacks;

            gun.ammo = stacks;
            gun.attackSpeed = (float)Math.Pow(.95f, stacks);
            gun.projectileSpeed = (float)Math.Pow(1.05f, stacks);
            gun.reflects = stacks / 2;
            gun.damage = (float)Math.Pow(1.05f, stacks);
            gun.bursts = stacks / 10;
            gun.numberOfProjectiles = stacks / 5;

            cardInfo.cardStats = MomentumTracker.GetOffensiveMomentumStats(stacks);
            tracker.updated = true;

            UnityEngine.Debug.Log("[WWC][Card] Unstoppable Force Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //if (!(GM_Test.instance != null && GM_Test.instance.gameObject.activeInHierarchy))
            {
                MomentumTracker.stacks = 0;
            }
        }
        protected override string GetTitle()
        {
            return $"Unstoppable Force ({MomentumTracker.stacks} Stacks)";
        }
        public override bool GetEnabled()
        {
            return false;
        }
    }

    public class MomentumCard_Mono : MonoBehaviour
    {
        private TextMeshProUGUI description;
        private TextMeshProUGUI cardName;
        public CardInfo card;
        public bool updated = false;
        public bool builtCard = false;
        public int stacks;
        public string title;

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
                cardName.text = title;
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
    public class MomentumCleanup_Mono : MonoBehaviour
    {
        public Player player;
        public void CleanUp()
        {
            this.ExecuteAfterFrames(10, MomentumTracker.RemoveBaseCards);
        }
    }

    public class MomentumTracker
    {
        public static int stacks = 0;
        public static Dictionary<int, CardInfo> createdOffenseCards = new Dictionary<int, CardInfo>();
        public static bool building = false;

        public static void AddOffenseCard(CardInfo cardInfo, Player player)
        {
            createdOffenseCards.Add(MomentumTracker.stacks, cardInfo); 
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo);
            building = false;
            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, cardInfo, false, "", 2f, 2f, true);
            
        }

        public static void RemoveBaseCards()
        {
            foreach (var player in PlayerManager.instance.players)
            {
                var momentumCards = player.data.currentCards.Where(cardInfo => cardInfo.cardName == "Unstoppable Force").ToList().Count;
                momentumCards += 0;
                if (momentumCards > 0)
                {
                    for (int i = player.data.currentCards.Count - 1; i >= 0; i--)
                    {
                        if (player.data.currentCards[i].cardName == "Unstoppable Force")
                        {
                            ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, i);
                            break;
                        }
                    }
                }
            }
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
    }
}
