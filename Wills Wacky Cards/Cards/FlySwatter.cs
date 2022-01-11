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
    class FlySwatter : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.AddComponent<FlySwatter_Mono>();

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetComponent<FlySwatter_Mono>();
            UnityEngine.GameObject.Destroy(mono);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Fly Swatter";
        }
        protected override string GetDescription()
        {
            return "Foes above you are swatted out of the air.";
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
                //new CardInfoStat()
                //{
                //    positive = true,
                //    stat = "Effect",
                //    amount = "No",
                //    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                //}
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.PoisonGreen;
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
    public class FlySwatter_Mono : Hooked_Mono
    {
        private CharacterData data;
        private Player player;

        private void Start()
        {
            HookedMonoManager.instance.hookedMonos.Add(this);
            data = GetComponentInParent<CharacterData>();
            player = data.player;
        }

        private void Update()
        {
            // If we're dead, get out
            if (player.data.dead)
            {
                return;
            }

            // If we're not simulated, get out
            if (!(bool)data.playerVel.GetFieldValue("simulated"))
            {
                return;
            }

            foreach (var person in PlayerManager.instance.players.Where(p => p.teamID != player.teamID && PlayerManager.instance.CanSeePlayer(player.transform.position, p).canSee && Vector2.Distance(p.transform.position, player.transform.position) > 4f).ToArray())
            {

                if (Vector2.Angle(Vector2.up, (person.transform.position - player.transform.position).normalized) <= 45f)
                {
                    person.data.playerVel.AddForce(Vector2.down * 10 * (float)player.data.playerVel.GetFieldValue("mass"), ForceMode2D.Impulse);
                }
            }
        }

        private bool PointIsInAngle(Vector2 startPos, Vector2 direction, float angle, Vector2 point)
        {
            // Get the angle from the start position to the point
            var pointDir = (point - startPos).normalized;

            // Construct the boundary vectors
            var dirAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var a = (new Vector2(Mathf.Cos((dirAngle + angle / 2f) * Mathf.Deg2Rad), Mathf.Sin((dirAngle + angle / 2f) * Mathf.Deg2Rad))).normalized;
            var b = (new Vector2(Mathf.Cos((dirAngle - angle / 2f) * Mathf.Deg2Rad), Mathf.Sin((dirAngle - angle / 2f) * Mathf.Deg2Rad))).normalized;

            // If the angle between the left and right boundaries is less than the angle, then it's within them.
            if (Vector2.Angle(a, pointDir) < angle && Vector2.Angle(b, pointDir) < angle)
            {
                return true;
            }

            return false;
        }

        public override void OnBattleStart()
        {

        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
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