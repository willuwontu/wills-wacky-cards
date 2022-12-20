using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    class Boomerang : CustomCard
    {
        private static GameObject boomerangObject = null;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            if (!boomerangObject)
            {
                boomerangObject = new GameObject("A_Boomerang", new Type[] { typeof(BoomerangBullet_Mono) });
                DontDestroyOnLoad(boomerangObject);
            }

            gun.objectsToSpawn = new ObjectsToSpawn[] { new ObjectsToSpawn { AddToProjectile = boomerangObject } };
            gun.ammo = 3;
            gun.bulletDamageMultiplier = 1.45f;
            gun.gravity = 0f;
            gun.attackSpeed = 2f;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("TRT_Enabled") };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            // Edits values on player when card is selected
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Drives me crazy
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Boomerang";
        }
        protected override string GetDescription()
        {
            return "Better get ready to catch it once it comes back.";
        }
        protected override GameObject GetCardArt()
        {
            GameObject art;

            try
            {
                art = WillsWackyManagers.WillsWackyManagers.instance.WWMAssets.LoadAsset<GameObject>("C_Boomerang");
            }
            catch
            {
                art = null;
            }

            return art;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Damage",
                    amount = "+45%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Ammo",
                    amount = "+3",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullet Gravity",
                    amount = "No",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Attack Speed",
                    amount = "-100%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }

            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
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
    public class BoomerangBullet_Mono : UnityEngine.MonoBehaviour
    {
        MoveTransform move;
        Vector3 startPos;
        float creationTime;

        float distanceMult = 0.025f;
        float speedMult = 0.01f;
        int layerMask;

        private void Start()
        {
            if (gameObject.transform.parent == null)
            {
                return;
            }
            move = base.GetComponentInParent<MoveTransform>();
            creationTime = Time.time;
            startPos = base.transform.position;
            layerMask = ~LayerMask.GetMask("BackgroundObject", "Player", "Projectile");

            var remove = base.GetComponentInParent<RemoveAfterSeconds>();
            remove.enabled = false;
        }

        private void Update()
        {
            if (gameObject.transform.parent == null)
            {
                return;
            }
            if (!move)
            {
                move = base.GetComponentInParent<MoveTransform>();
            }
            if (startPos != null && move != null)
            {
                var distance = Vector2.Distance(base.transform.root.position, startPos);

                var direction = (startPos - base.transform.position).normalized;

                move.velocity += Vector3.ClampMagnitude(direction * 1000, distance * distanceMult + move.velocity.magnitude * speedMult);

                // Check for Floor and ceiling to give ourselves a nudge.
                RaycastHit2D hit = Physics2D.Raycast(base.transform.position, Vector3.down, 1f, layerMask);

                if (hit)
                {
                    move.velocity += Vector3.up * move.gravity * Mathf.Clamp(1f / hit.distance, 0, 100f);
                }

                hit = Physics2D.Raycast(base.transform.position, Vector2.up, 1f, layerMask);

                if (hit)
                {
                    move.velocity += Vector3.down * move.gravity * Mathf.Clamp(1f / hit.distance, 0, 100f);
                }
            }
        }
    }
}