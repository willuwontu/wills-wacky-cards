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
using Photon.Pun;
using Sonigon;

namespace WWC.Cards
{
    class Banishment : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            block.InvokeMethod("ResetStats");
            block.cdMultiplier = 1.1f;

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<Banishment_Mono>();
            mono.duration += 3f;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetOrAddComponent<Banishment_Mono>();
            UnityEngine.GameObject.Destroy(mono);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Banishment";
        }
        protected override string GetDescription()
        {
            return "Blocking an enemy's shots banishes them to the shadows, slowing and silencing them temporarily.";
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
                    stat = "Block Cooldown",
                    amount = "+10%",
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
    public class Banishment_Mono : Hooked_Mono
    {
        public float duration = 0f;

        private Dictionary<Player, float> banished = new Dictionary<Player, float>();

        private Camera camera;
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
            player = data.player;
            weaponHandler = data.weaponHandler;
            gun = weaponHandler.gun;
            gunAmmo = gun.GetComponentInChildren<GunAmmo>();
            block = data.block;
            stats = data.stats;
            health = data.healthHandler;
            gravity = player.GetComponent<Gravity>();
            block.BlockProjectileAction += OnBlockProjectile;
            camera = gameObject.AddComponent<Camera>();
            camera.enabled = false;
        }

        private void Update()
        {
            if (banished.Count > 0)
            {
                var players = banished.Keys.ToArray();

                foreach (var person in players)
                {
                    banished[person] -= Time.deltaTime;

                    if (banished[person] <= 0f)
                    {
                        StopBanish(person);
                    }
                }
            }
        }

        private void OnBlockProjectile(GameObject projectile, Vector3 forward, Vector3 hitPos)
        {
            var bullet = projectile.GetComponent<ProjectileHit>();
            var attacker = bullet.ownPlayer;

            var silenceHandler = attacker.GetComponent<SilenceHandler>();
            silenceHandler.RPCA_AddSilence(duration);

            StartBanish(attacker, duration);

            //this.photonView.RPC(nameof(RPCA_StartBanish), RpcTarget.All, attacker.playerID);
        }

        [PunRPC]
        private void RPCA_StartBanish(int attackerID)
        {
            var attacker = PlayerManager.instance.GetPlayerWithID(attackerID);

            var silenceHandler = attacker.GetComponent<SilenceHandler>();
            silenceHandler.RPCA_AddSilence(duration);

            StartBanish(attacker, duration);
        }

        private void StartBanish(Player attacker, float f)
        {
            if (banished.Keys.Contains(attacker))
            {
                if (!(banished[attacker] > 0f))
                {
                    attacker.data.stats.GetAdditionalData().isBanished = true;
                    if (attacker.data.view.IsMine)
                    {
                        camera.enabled = true;
                    }
                }
                banished[attacker] += f;
            }
            else
            {
                attacker.data.stats.GetAdditionalData().isBanished = true;
                banished.Add(attacker, f);
                
                if (attacker.data.view.IsMine)
                {
                    camera.enabled = true;
                }
            }
            var banish = attacker.gameObject.GetOrAddComponent<BanishedPlayer_Mono>();
            banish.duration += f;
            banish.go = true;
        }

        private void StopBanish(Player attacker)
        {
            attacker.data.stats.GetAdditionalData().isBanished = false;
            var banish = attacker.gameObject.GetOrAddComponent<BanishedPlayer_Mono>();
            UnityEngine.GameObject.Destroy(banish);
            banished.Remove(attacker);
            if (attacker.data.view.IsMine)
            {
                camera.enabled = false;
            }
        }

        public override void OnPointStart()
        {

        }

        public override void OnPointEnd()
        {
            var people = banished.Keys.ToArray();
            foreach (var person in people)
            {
                banished[person] = 0;
                person.data.stats.GetAdditionalData().isBanished = false;
                StopBanish(person);
            }

            banished.Clear();
            banished = new Dictionary<Player, float>();
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            block.BlockProjectileAction -= OnBlockProjectile;

            UnityEngine.GameObject.Destroy(camera);
            HookedMonoManager.instance.hookedMonos.Remove(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }

    [DisallowMultipleComponent]
    public class BanishedPlayer_Mono : Hooked_Mono
    {
        public float duration = 0f;
        public bool go = false;

        public float slow = 0.7f;
        private Dictionary<Player, float> banished = new Dictionary<Player, float>();

        private CharacterData data;
        private Player player;
        private CharacterStatModifiers stats;

        private void Start()
        {
            HookedMonoManager.instance.hookedMonos.Add(this);
            data = GetComponentInParent<CharacterData>();
            player = data.player;
            stats = data.stats;
            stats.movementSpeed *= slow;
        }

        private void Update()
        {
            if (go)
            {
                duration -= Time.deltaTime;
            }

            if (duration <= 0f)
            {
                UnityEngine.GameObject.Destroy(this);
            }
        }

        public override void OnPointEnd()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            stats.movementSpeed /= slow;
            HookedMonoManager.instance.hookedMonos.Remove(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}
