using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WillsWackyManagers.Utils;
using WWC.Extensions;
using WWC.MonoBehaviours;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    class GhostlyBody : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseSpawnerCategory, CurseEater.CurseEaterClass, CustomCardCategories.instance.CardCategory("Ghost Body") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("Runic Wards") };

            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(20, () => CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); }));
            player.gameObject.GetOrAddComponent<GhostBody_Mono>();
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Ghostly Body";
        }
        protected override string GetDescription()
        {
            return "As you gain more curses, your body begins to merge with the shadows and become resilient to damage. If you have 16 curses, you gain the power to fly.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Damage Reduction per curse",
                    amount = "+5%",
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
            return CardThemeColor.CardThemeColorType.EvilPurple;
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
}

namespace WWC.MonoBehaviours
{
    [DisallowMultipleComponent]
    class GhostBody_Mono : Hooked_Mono
    {
        private bool changedGravity = false;
        private float prevGravity;
        private float multiplier = 0f;
        private bool increased = false;
        private int curseCount = 0;
        private ModdingUtils.MonoBehaviours.InAirJumpEffect flight;

        private CharacterData data;
        private Player player;
        private CharacterStatModifiers stats;
        private Gravity gravity;

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
                    stats = data.stats;
                    gravity = player.GetComponent<Gravity>();
                }

            }
        }

        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName.ToLower() == "Ghostly Body".ToLower())
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

        public override void OnPointStart()
        {
            CheckIfValid();
            if (!increased)
            {
                increased = true;
                var curses = CurseManager.instance.GetAllCursesOnPlayer(player).Count();
                curseCount = curses;
                multiplier = 0.05f * curses;
                stats.GetAdditionalData().DamageReduction += multiplier;

                if (curseCount < 16 && gravity.gravityForce != 0f)
                {
                    gravity.gravityForce *= (1f - (1f / 16f * (float)Mathf.Clamp(curseCount, 0, 16)));
                }
                else if (curseCount >= 16 && gravity.gravityForce != 0f)
                {
                    prevGravity = gravity.gravityForce;
                    gravity.gravityForce = 0f;
                    changedGravity = true;
                }

                if (multiplier >= 0.8f)
                {

                    flight = player.gameObject.AddComponent<ModdingUtils.MonoBehaviours.InAirJumpEffect>();
                    flight.SetJumpMult(0.1f);
                    flight.AddJumps(1000000);
                    flight.SetCostPerJump(1);
                    flight.SetContinuousTrigger(true);
                    flight.SetResetOnWallGrab(true);
                    flight.SetInterval(0.1f);
                }
            }
        }

        public override void OnPointEnd()
        {
            if (increased)
            {
                stats.GetAdditionalData().DamageReduction -= multiplier;

                if (curseCount < 16 && gravity.gravityForce != 0f)
                {
                    gravity.gravityForce /= (1f - (1f / 16f * (float)Mathf.Clamp(curseCount, 0, 16)));
                }
                else if (changedGravity)
                {
                    gravity.gravityForce = prevGravity;
                    changedGravity = false;
                }

                if (multiplier >= 0.8f)
                {
                    UnityEngine.GameObject.Destroy(flight);
                }

                increased = false;
            }
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            OnPointEnd();
            HookedMonoManager.instance.hookedMonos.Remove(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}
