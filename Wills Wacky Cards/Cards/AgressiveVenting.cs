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
    class AgressiveVenting : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.attackSpeed = 20f;
            gun.reloadTimeAdd = 0.25f;
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            gun.attackSpeedMultiplier = 0.5f;
            player.gameObject.AddComponent<AgressiveVenting_Mono>();
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetComponent<AgressiveVenting_Mono>();
            UnityEngine.GameObject.Destroy(mono);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Agressive Venting";
        }
        protected override string GetDescription()
        {
            return "What's this? Redirecting the heat from your barrel can hurt enemies and help you shoot faster? Sounds like a plan to me.";
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
                    stat = "Attack Speed",
                    amount = "+100%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Reload Time",
                    amount = "+0.25s",
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
    [DisallowMultipleComponent]
    public class AgressiveVenting_Mono : Hooked_Mono
    {
        private GameObject ventingVisual = null;
        private LineEffect effectRadius = null;
        private List<LineEffect> radiatingVisuals = new List<LineEffect>();
        private float speed = 10f;
        private int layerMask = ~LayerMask.GetMask("BackgroundObject");
        private int checkMask = ~LayerMask.GetMask("BackgroundObject", "Player", "Projectile", "PlayerObjectCollider");
        private float gunDamageDealtOver = 1/10f;

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

            AddVentingVisual();
        }

        private void Update()
        {
            var activate = gun.IsReady(0f);

            ventingVisual.SetActive(!activate);

            if (activate)
            {
                return;
            }

            effectRadius.radius = (gun.attackSpeed / gun.attackSpeedMultiplier) * 3f * (1- gun.ReadyAmount());

            if (!(radiatingVisuals.Count() > 0) || (radiatingVisuals.Count() > 0 && radiatingVisuals[radiatingVisuals.Count()-1].radius > 1f))
            {
                radiatingVisuals.Add(AddRadiatingVisual());
            }

            var radius = (float)effectRadius.InvokeMethod("GetRadius");
            var hits = Physics2D.CircleCastAll(player.transform.position, radius, data.hand.transform.forward, 0.0f, layerMask);

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<Damagable>())
                {
                    var check = Physics2D.Linecast(player.transform.position, hit.point, checkMask);
                    if (check)
                    {
                        UnityEngine.Debug.Log($"{check.collider.gameObject.name} != {hit.collider.gameObject.name}");
                    }
                    if (!check || check.collider.gameObject == hit.collider.gameObject)
                    {
                        var damagable = hit.collider.gameObject.GetComponent<Damagable>();
                        if (damagable)
                        {
                            if (damagable.GetComponent<Player>() && damagable.GetComponent<Player>().teamID == player.teamID)
                            {
                                continue;
                            }

                            damagable.CallTakeDamage(((hit.point - (Vector2) player.transform.position).normalized * gun.damage * (1 - gun.ReadyAmount()) * (hit.distance / radius)), hit.point, null, player);
                        }
                    }
                }


                //if (PlayerManager.instance.CanSeePlayer(hit.gameObject.transform.position, player).canSee)
                //{



                //    var damagable = hit.GetComponentInParent<Damagable>();
                //    if (damagable)
                //    {
                //        if (damagable.GetComponent<Player>() && damagable.GetComponent<Player>().teamID == player.teamID)
                //        {
                //            continue;
                //        }

                //        damagable.CallTakeDamage(((hit.gameObject.transform.position - player.transform.position).normalized * gun.damage * (Time.deltaTime / gunDamageDealtOver) * (1 - gun.ReadyAmount()) * (Vector3.Distance(hit.gameObject.transform.position, player.transform.position) / (float)effectRadius.InvokeMethod("GetRadius"))), hit.gameObject.transform.position, null, player);
                //    }
                //}
            }

            var visuals = radiatingVisuals.ToArray();

            foreach (var visual in visuals)
            {
                if (visual.radius > effectRadius.radius)
                {
                    radiatingVisuals.Remove(visual);
                    UnityEngine.GameObject.Destroy(visual.gameObject);
                }
                else
                {
                    visual.radius += TimeHandler.deltaTime * speed;
                }
            }

            //gun.sinceAttack += TimeHandler.deltaTime * gun.attackSpeedMultiplier;
        }

        public void AddVentingVisual()
        {
            if (ventingVisual != null) { return; }
            var card = CardChoice.instance.cards.First(c => c.name.Equals("ChillingPresence"));
            var statMods = card.gameObject.GetComponentInChildren<CharacterStatModifiers>();
            ventingVisual =  Instantiate(statMods.AddObjectToPlayer.GetComponentInChildren<LineEffect>().gameObject, player.transform);
            ventingVisual.name = "A_AgressiveVenting";

            effectRadius = ventingVisual.gameObject.GetComponent<LineEffect>();
            effectRadius.segments = 100;
            effectRadius.effects[0].mainCurveMultiplier = 1f;
            effectRadius.effects[1].mainCurveMultiplier = 1f;
            effectRadius.effects[0].mainCurveScrollSpeed = 3f;

            var renderer = effectRadius.gameObject.GetComponent<LineRenderer>();
            renderer.endColor = renderer.startColor = new Color(1f, 0.368f, 0f, 1f);
            renderer.startWidth = renderer.endWidth = 0.1f;
        }

        private LineEffect AddRadiatingVisual()
        {
            var card = CardChoice.instance.cards.First(c => c.name.Equals("ChillingPresence"));
            var statMods = card.gameObject.GetComponentInChildren<CharacterStatModifiers>();
            var radiateObj = Instantiate(statMods.AddObjectToPlayer.GetComponentInChildren<LineEffect>().gameObject, ventingVisual.transform);
            var lineEffect = radiateObj.GetComponent<LineEffect>();
            lineEffect.radius = 0f;

            var renderer = lineEffect.gameObject.GetComponent<LineRenderer>();
            renderer.endColor = renderer.startColor = new Color(1f, 0.4f, 0f, 1f);
            renderer.startWidth = renderer.endWidth = 0.1f;
            return lineEffect;
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            HookedMonoManager.instance.hookedMonos.Remove(this);
            UnityEngine.GameObject.Destroy(ventingVisual);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}