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
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;

namespace WWC.Cards
{
    class AggressiveVenting : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.attackSpeed = 0.5f;
            gun.reloadTimeAdd = 0.25f;
            cardInfo.allowMultiple = false;

            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("TRT_Enabled") };
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.gameObject.AddComponent<AggressiveVenting_Mono>();
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            var mono = player.gameObject.GetComponent<AggressiveVenting_Mono>();
            UnityEngine.GameObject.Destroy(mono);
            WillsWackyCards.instance.DebugLog($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Aggressive Venting";
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
                    positive = false,
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
    public class AggressiveVenting_Mono : MonoBehaviour, IGameStartHookHandler
    {
        private GameObject ventingVisual = null;
        private LineEffect effectRadius = null;
        private List<LineEffect> radiatingVisuals = new List<LineEffect>();
        private float speed = 10f;
        private int layerMask = ~LayerMask.GetMask("BackgroundObject");
        private int checkMask = ~LayerMask.GetMask("BackgroundObject", "Player", "Projectile", "PlayerObjectCollider");
        private float gunDamageDealtOver = 1.5f;

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
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
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

        private float CalculateDamage(float damage)
        {
            return (damage / 2f + 1f / 2f) * 55;
        }

        private float CalculateRadius(float attackSpeed)
        {
            return (attackSpeed / 10) + 4.9f + (attackSpeed > 0.3f ? attackSpeed * 3f : 0f);
        }

        private void FixedUpdate()
        {
            var activate = gun.IsReady(0f);

            ventingVisual.SetActive(!activate);

            if (activate)
            {
                return;
            }

            effectRadius.radius = CalculateRadius(gun.attackSpeed / gun.attackSpeedMultiplier) * (1- gun.ReadyAmount());

            var radius = (float)effectRadius.InvokeMethod("GetRadius");
            var hits = Physics2D.OverlapCircleAll(player.transform.position, radius);


            foreach (var hit in hits)
            {
                if (hit.gameObject.GetComponent<Damagable>())
                {
                    var check = Physics2D.Linecast(player.transform.position, hit.gameObject.transform.position, checkMask);
                    if (!check || check.collider.gameObject == hit.gameObject)
                    {
                        var damagable = hit.gameObject.GetComponent<Damagable>();
                        if (damagable)
                        {
                            if (damagable.GetComponent<Player>() && damagable.GetComponent<Player>().teamID == player.teamID)
                            {
                                continue;
                            }

                            damagable.CallTakeDamage((((Vector2)hit.gameObject.transform.position - (Vector2)player.transform.position).normalized * CalculateDamage(gun.damage * gun.bulletDamageMultiplier) * (Time.deltaTime / gunDamageDealtOver) * (1 - gun.ReadyAmount()) * (1 - (Vector2.Distance((Vector2)hit.gameObject.transform.position, (Vector2)player.transform.position) / radius))), (check ? check.point : (Vector2)hit.gameObject.transform.position), null, player);
                        }
                    } 
                }
            }

            var visuals = radiatingVisuals.ToArray();

            foreach (var visual in visuals)
            {
                if (!visual.gameObject.activeSelf)
                {
                    visual.gameObject.SetActive(true);
                }

                if (visual.radius > effectRadius.radius)
                {
                    visual.radius = 0f;
                    radiatingVisuals.Remove(visual);
                    UnityEngine.GameObject.Destroy(visual.gameObject);
                }
                else
                {
                    visual.radius += TimeHandler.deltaTime * speed;
                }
            }

            if (!(radiatingVisuals.Count() > 0) || (radiatingVisuals.Count() > 0 && radiatingVisuals[radiatingVisuals.Count() - 1].radius > 1f))
            {
                radiatingVisuals.Add(AddRadiatingVisual());
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
            effectRadius.segments = 200;
            effectRadius.effects[0].mainCurveMultiplier = .5f;
            effectRadius.effects[1].mainCurveMultiplier = 1f;
            effectRadius.effects[0].mainCurveScrollSpeed = 3f;
            effectRadius.effects[1].mainCurveScrollSpeed = 5f;

            var renderer = effectRadius.gameObject.GetComponent<LineRenderer>();
            renderer.endColor = renderer.startColor = new Color(1f, 0.368f, 0f, 1f);
            renderer.startWidth = renderer.endWidth = 0.1f;
            renderer.positionCount = 200;
        }

        private LineEffect AddRadiatingVisual()
        {
            var card = CardChoice.instance.cards.First(c => c.name.Equals("ChillingPresence"));
            var statMods = card.gameObject.GetComponentInChildren<CharacterStatModifiers>();
            var radiateObj = Instantiate(statMods.AddObjectToPlayer.GetComponentInChildren<LineEffect>().gameObject, ventingVisual.transform);
            var lineEffect = radiateObj.GetComponent<LineEffect>();
            lineEffect.radius = 0f;
            lineEffect.segments = 100;
            lineEffect.effects[0].mainCurveMultiplier = .5f;
            lineEffect.effects[1].mainCurveMultiplier = .5f;
            lineEffect.effects[0].mainCurveScrollSpeed = 3f;

            var renderer = lineEffect.gameObject.GetComponent<LineRenderer>();
            renderer.endColor = renderer.startColor = new Color(1f, 0.4f, 0f, 1f);
            renderer.startWidth = renderer.endWidth = 0.1f;

            radiateObj.SetActive(false);
            return lineEffect;
        }

        public void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            InterfaceGameModeHooksManager.instance.RemoveHooks(this);
            UnityEngine.GameObject.Destroy(ventingVisual);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}