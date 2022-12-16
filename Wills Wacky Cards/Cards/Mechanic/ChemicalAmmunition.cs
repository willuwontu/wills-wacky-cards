using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using WWC.MonoBehaviours;
using WWC.Interfaces;
using ModdingUtils.RoundsEffects;
using UnityEngine;
using ClassesManagerReborn.Util;

namespace WWC.Cards
{
    class ChemicalAmmunition : CustomCard
    {
        public override void Callback()
        {
            gameObject.GetOrAddComponent<ClassNameMono>().className = MechanicClass.name;
        }
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            cardInfo.allowMultiple = false;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.gameObject.AddComponent<ChemicalBullets_Mono>();

            gun.projectileColor = new Color(0.2f, 0.7f, 0.2f, 1f);

            var upgrader = player.GetComponentInChildren<MechanicUpgrader>();

            if (upgrader)
            {
                upgrader.gunStatModifier.damage_mult += 0.30f;
                upgrader.gunStatModifier.reflects_add += 1;
                upgrader.upgradeTimeAdd += 1.5f;
            }

            ObjectsToSpawn item = ((GameObject)Resources.Load("0 cards/Mayhem")).GetComponent<Gun>().objectsToSpawn[0];
            List<ObjectsToSpawn> list = gun.objectsToSpawn.ToList<ObjectsToSpawn>();
            list.Add(item);
            gun.objectsToSpawn = list.ToArray();

            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        internal static CardInfo card = null;
        protected override string GetTitle()
        {
            return "Chemical Ammunition";
        }
        protected override string GetDescription()
        {
            return $"A little ClF3 never hurt anyone, right? You may want to aim away from yourself.";
        }
        protected override GameObject GetCardArt()
        {
            GameObject art;

            try
            {
                art = WillsWackyManagers.WillsWackyManagers.instance.WWWMAssets.LoadAsset<GameObject>("C_MechanicCardPlaceHolder");
            }
            catch
            {
                art = null;
            }

            return art;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "DMG per Upgrade",
                    amount = "+30%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bounce per Upgrade",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Upgrade Time",
                    amount = "+1.5s",
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
    public class ChemicalBullets_Mono : HitSurfaceEffect
    {
        private void Start()
        {
            data = GetComponent<CharacterData>();
            player = data.player;
            block = data.block;
            gun = data.weaponHandler.gun;
        }

        public override void Hit(Vector2 position, Vector2 normal, Vector2 velocity)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(((GameObject)Resources.Load("0 cards/Demonic pact")).GetComponent<Gun>().objectsToSpawn[0].effect);
            gameObject.transform.position = position;
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            gameObject.name = "Explosion";
            gameObject.AddComponent<RemoveAfterSeconds>().seconds = 2f;
            UnityEngine.GameObject.Destroy(gameObject.GetComponent<Explosion>());
            ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].startColor = new Color(0.2f, 1f, 0.2f, 1f);
            }
            foreach (ParticleSystemRenderer particleSystemRenderer in gameObject.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                particleSystemRenderer.material.color = new Color(0.2f, 1f, 0.2f, 1f);
                particleSystemRenderer.sharedMaterial.color = new Color(0.2f, 1f, 0.2f, 1f);
            }
            Material[] componentsInChildren3 = gameObject.GetComponentsInChildren<Material>();
            for (int i = 0; i < componentsInChildren3.Length; i++)
            {
                componentsInChildren3[i].color = new Color(0.2f, 1f, 0.2f, 1f);
            }

            var cols = Physics2D.OverlapCircleAll(position, 2.5f).Where(col => (col.GetComponent<ProjectileCollision>() || col.GetComponent<Rigidbody2D>() || col.GetComponent<Player>())).ToArray();

            foreach (var col in cols)
            {
                var acid = col.gameObject.AddComponent<ChemicalBurns_Mono>();

                RaycastHit2D hit = Physics2D.LinecastAll(position, (Vector2) col.transform.position).Where(hitCol => hitCol.collider == col).First();

                acid.damage = 55f * gun.damage * gun.bulletDamageMultiplier * (1f - (hit.distance / 2.5f));
            }
        }

        public Block block;

        public Player player;

        public CharacterData data;

        public Gun gun;
    }

    public class ChemicalBurns_Mono : MonoBehaviour, IPointEndHookHandler
    {
        private float lifetime = 4f;
        private Dictionary<SpriteRenderer, Color> originalColors;
        private Color acidColor = new Color(0.3f, 0.7f, 0.3f, 1f);

        public float damage = 55f;
        public bool activated = false;

        private void OnDestroy()
        {
            if (this.GetComponent<Rigidbody2D>() && activated)
            {
                foreach (var sr in this.originalColors.Keys)
                {
                    sr.color = originalColors[sr];
                }
            }
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
        }
        private void Start()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);

            var burns = this.GetComponents<ChemicalBurns_Mono>();

            if (burns.Length == 1)
            {
                this.activated = true;
                if (this.GetComponent<Rigidbody2D>())
                {
                    this.originalColors = this.GetComponentsInChildren<SpriteRenderer>().ToDictionary(sr=> sr, sr => sr.color);

                    foreach (var sr in this.originalColors.Keys)
                    {
                        sr.color = acidColor;
                    }
                }
            }
            bool flag = false;
            int num = -1;
            for (int i = 0; i < burns.Length; i++)
            {
                if (burns[i].activated)
                {
                    flag = true;
                    num = i;
                }
            }
            if (flag)
            {
                if (burns[num] != this)
                {
                    burns[num].lifetime += this.lifetime;
                    UnityEngine.GameObject.Destroy(this);
                }
            }
            else
            {
                this.activated = true;
                if (this.GetComponent<Rigidbody2D>())
                {
                    this.originalColors = this.GetComponentsInChildren<SpriteRenderer>().ToDictionary(sr => sr, sr => sr.color);

                    foreach (var sr in this.originalColors.Keys)
                    {
                        sr.color = acidColor;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (this.GetComponent<Player>())
            {
                HandlePlayer(this.GetComponent<Player>());
            }
            else if (this.GetComponent<Rigidbody2D>())
            {
                HandleBox(this.GetComponent<Rigidbody2D>());
            }
            else if (this.GetComponentInParent<ProjectileHit>())
            {
                HandleBullet(this.GetComponentInParent<ProjectileHit>());
            }
            else
            {
                UnityEngine.GameObject.Destroy(this);
            }
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                UnityEngine.GameObject.Destroy(this);
            }
        }

        public void HandlePlayer(Player player)
        {

            player.data.healthHandler.TakeDamageOverTime(Vector2.up * damage, Vector2.zero, 4f, 0.1f, new Color(0, 0.8f, 0, 1f));
            UnityEngine.GameObject.Destroy(this);
        }

        public void HandleBox(Rigidbody2D rb)
        {
            if (rb.isKinematic)
            {
                return;
            }

            if (rb.GetComponent<DamagableEvent>())
            {
                rb.GetComponent<DamagableEvent>().CallTakeDamage(Vector2.up * damage * Time.deltaTime / 4f, Vector2.zero);
            }
            else
            {
                rb.gameObject.transform.localScale = new Vector3(rb.gameObject.transform.localScale.x * 0.9985f, rb.gameObject.transform.localScale.y * 0.9985f, rb.gameObject.transform.localScale.z);

                if (((Vector2)rb.gameObject.transform.localScale).x < 0.05f || ((Vector2)rb.gameObject.transform.localScale).y < 0.05f)
                {
                    Destroy(rb.gameObject);
                }
            }
        }

        public void HandleBullet(ProjectileHit projectileHit)
        {
            projectileHit.GetComponentInChildren<ProjectileCollision>().TakeDamage(damage * Time.deltaTime / 4f);
            projectileHit.damage -= damage * Time.deltaTime / 5f;
        }

        public void OnPointEnd()
        {
            UnityEngine.GameObject.Destroy(this);
        }
    }
}