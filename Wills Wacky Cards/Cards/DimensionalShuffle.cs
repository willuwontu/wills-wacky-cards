using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using Photon.Pun;

namespace WWC.Cards
{
    class DimensionalShuffle : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            statModifiers.health = 0.7f;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<DimensionalShuffle_Mono>();
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<DimensionalShuffle_Mono>();
            UnityEngine.GameObject.Destroy(mono);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Dimensional Shuffle";
        }
        protected override string GetDescription()
        {
            return "When you block, each player's position is randomly swapped to another's.";
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
                    positive = false,
                    stat = "HP",
                    amount = "-30%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Ability Cooldown",
                    amount = "10s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.TechWhite;
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
    public class DimensionalShuffle_Mono : Hooked_Mono
    {
        private float lastUsed;
        private bool canTrigger = true;
        private float cooldown = 10f;

        private CharacterData data;
        private Player player;
        private Block block;
        int layerMask;

        private void Start()
        {
            HookedMonoManager.instance.hookedMonos.Add(this);
            data = GetComponentInParent<CharacterData>();
            player = data.player;
            block = data.block;
            block.SuperFirstBlockAction += OnBlock;
            layerMask = ~LayerMask.GetMask("BackgroundObject", "Player", "Projectile");
        }

        private void Update()
        {
            if (!canTrigger)
            {
                if ((Time.time >= lastUsed + cooldown))
                {
                    canTrigger = true;
                }
            }
        }

        private void OnBlock(BlockTrigger.BlockTriggerType blockTrigger)
        {
            if (this.photonView.IsMine && canTrigger)
            {
                var livingPlayers = PlayerManager.instance.players.Where((person) => !person.data.dead).ToArray();
                var playerPositions = livingPlayers.Select((person) => person.transform.position).ToList();

                livingPlayers.Shuffle();

                for (int index = 0; index < livingPlayers.Count(); index++)
                {
                    var person = livingPlayers[index];

                    var angle = UnityEngine.Random.Range(0f, 360f);
                    var distance = player.transform.localScale.x * 2f;
                    var direction = (new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad))).normalized;
                    Vector3 destination = playerPositions[index];
                    var hit = Physics2D.Raycast(destination, direction, distance, layerMask);
                    var bounces = 0;

                    while (hit && distance >= 0f && bounces < 1000)
                    {
                        bounces++;
                        distance -= hit.distance;
                        destination = hit.point;
                        direction = Vector2.Reflect(direction, hit.normal);
                        hit = Physics2D.Raycast(destination, direction, distance, layerMask);

                    }

                    destination += (Vector3)Vector2.ClampMagnitude((direction.normalized * distance), distance);

                    playerPositions[index] = destination;
                }

                this.photonView.RPC(nameof(RPCA_NewPositions), RpcTarget.AllViaServer, livingPlayers.Select(person => person.playerID).ToArray(), playerPositions.ToArray());

                lastUsed = Time.time;
                canTrigger = false;
            }
        }

        [PunRPC]
        private void RPCA_NewPositions(int[] playerIDs, Vector3[] positions)
        {
            for (int index = 0; index < playerIDs.Count(); index++)
            {
                var playerID = playerIDs[index];
                var person = PlayerManager.instance.GetPlayerWithID(playerID);

                //var prevGrav = person.GetComponent<Gravity>().gravityForce;
                //person.GetComponent<Gravity>().gravityForce = 0;
                //person.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                //person.transform.position = positions[index];

                PlayerManager.instance.StartCoroutine(MovePlayer(person, positions[index]));
            }
        }

        private IEnumerator MovePlayer(Player person, Vector3 position)
        {
            yield return (IEnumerator) typeof(PlayerManager).InvokeMember("Move",
                    BindingFlags.Instance | BindingFlags.InvokeMethod |
                    BindingFlags.NonPublic, null, PlayerManager.instance, new object[] { person.data.playerVel, position });

            for (int i = 0; i < 2; i++)
            {
                person.gameObject.transform.position = position;
                yield return null;
            }

            yield break;
        }

        public override void OnBattleStart()
        {
            canTrigger = true;
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            block.BlockAction -= OnBlock;
            HookedMonoManager.instance.hookedMonos.Remove(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}