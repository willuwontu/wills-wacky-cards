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
using HarmonyLib;

namespace WWC.Cards
{
    class ShadowBullets : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            GameObject drill = null;

            drill = GameObject.Find("A_ShadowBullet");

            if (!drill)
            {
                var drillCard = UnboundLib.Utils.CardManager.cards.Values.Where((card) => card.cardInfo.cardName.ToLower() == "Drill Ammo".ToLower()).Select((card) => card.cardInfo).FirstOrDefault();

                var drillGun = drillCard.gameObject.GetComponent<Gun>();

                drill = Instantiate(drillGun.objectsToSpawn[0].AddToProjectile);
                drill.name = "A_ShadowBullet";

                var drillRay = drill.GetComponent<RayHitDrill>();

                drillRay.metersOfDrilling = 5000f;
                drillRay.speedModFlat = 0.9f;
                drillRay.speedMod = 0.05f; 
            }

            gun.objectsToSpawn = new ObjectsToSpawn[] { new ObjectsToSpawn { AddToProjectile = drill } };

            gun.unblockable = true;
            gun.bulletDamageMultiplier = 0.75f;
            gun.reloadTimeAdd = 0.75f;
            gun.attackSpeed = 1.4f;
            gun.gravity = 0f;
            gun.projectielSimulatonSpeed = .65f;

            cardInfo.allowMultiple = false;
            cardInfo.categories = new CardCategory[] { CurseManager.instance.curseSpawnerCategory, CurseEater.CurseEaterClass, CustomCardCategories.instance.CardCategory("Shadow Bullets") };
            cardInfo.blacklistedCategories = new CardCategory[] { CustomCardCategories.instance.CardCategory("Corrupted Ammunition") };

            //UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //gun.projectileColor = new Color(10f / 255f, 10f / 255f, 10f / 255f, 0.1f);
            var mono = player.gameObject.GetOrAddComponent<WWC.MonoBehaviours.ShadowBullets_Mono>();
            //WillsWackyCards.instance.ExecuteAfterFrames(20, () => CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); }));
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Shadow Bullets";
        }
        protected override string GetDescription()
        {
            return "";
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
                    stat = "Walls",
                    amount = "Pierce",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bullets",
                    amount = "Unblockable",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Damage",
                    amount = string.Format("-{0:F0}%", ((1f / 0.75f) - 1f) * 100f),
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Attack Speed",
                    amount = "-40%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload Time",
                    amount = "+0.75s",
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
    class ShadowBullets_Mono : Hooked_Mono
    {
        private CharacterData data;
        private Player player;
        private Gun gun;

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
                    gun = data.weaponHandler.gun;

                    gun.ShootPojectileAction += OnShootProjectileAction;
                }

            }
        }

        private void OnShootProjectileAction(GameObject obj)
        {
            var shadowColor = new Color(10f / 255f, 10f / 255f, 10f / 255f, 0f / 255f);

            ProjectileHit bullet = obj.GetComponent<ProjectileHit>();

            bullet.projectileColor = shadowColor;

            var spawnedAttack = obj.GetComponent<SpawnedAttack>();
            spawnedAttack.SetColor(shadowColor);

            this.ExecuteAfterFrames(1, () =>
            {
                foreach (Transform child in obj.transform)
                {
                    var particles = child.gameObject.GetComponentsInChildren<ParticleSystem>();
                    var renderers = child.gameObject.GetComponentsInChildren<ParticleSystemRenderer>();

                    foreach (var particle in particles)
                    {
                        var main = particle.main;
                        main.startColor = shadowColor;
                        var colorOverLifeTime = particle.colorOverLifetime;
                        Gradient gradient = new Gradient();

                        gradient.SetKeys(
                            new GradientColorKey[] { new GradientColorKey(shadowColor, 0f), new GradientColorKey(shadowColor, 1f) }, new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0f, 0f) }
                            );

                        colorOverLifeTime.color = new ParticleSystem.MinMaxGradient(gradient);

                        particle.gameObject.SetActive(false);
                    }

                    foreach (var renderer in renderers)
                    {

                        renderer.gameObject.SetActive(false);
                    }

                    var remover = obj.GetComponent<RemoveAfterSeconds>();
                    remover.seconds += 120;
                    //remover.enabled = false;
                }

                var colliderObj = obj.transform.Find("Collider").gameObject;
                var collider = colliderObj.GetComponent<ProjectileCollision>();

                collider.sparkObject = new GameObject();
                collider.sparkObject.transform.parent = collider.transform;
                collider.sparkObject.name = "E_BulletHitBullet_Dummy";
                //var remove = collider.sparkObject.AddComponent<RemoveAfterSeconds>();
                //remove.seconds = 0.1f;

            });

            obj.AddComponent<ShadowMovement_BulletMono>();
        }

        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName.ToLower() == "Shadow Bullets".ToLower())
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

        //public override void OnBattleStart()
        //{
        //    gun.spread = Mathf.Clamp(gun.spread, 0.01f, float.PositiveInfinity);
        //}

        public override void OnPointStart()
        {
            CheckIfValid();
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        private void OnDestroy()
        {
            gun.ShootPojectileAction -= OnShootProjectileAction;
            HookedMonoManager.instance.hookedMonos.Remove(this);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }
    public class ShadowMovement_BulletMono : MonoBehaviour
    {
        private Vector3 prevPosition;
        private Vector3 prevPoint = Vector3.zero;
        private float distanceTraveled = 0f;
        List<Vector3[]> curves = new List<Vector3[]>();
        private bool goToZero = false;

        private void Start()
        {
            prevPosition = base.transform.root.position;
            curves.Add(GenerateCurves());
        }

        private void Update()
        {
            distanceTraveled += Vector3.Distance(base.transform.root.position, prevPosition);
            float percent = distanceTraveled / curves[0][3].x;
            if (percent >= 1f)
            {
                distanceTraveled -= curves[0][3].x;
                curves.Add(GenerateCurves());
                curves.Remove(curves[0]);
                percent = distanceTraveled / curves[0][3].x;
            }
            Vector3 point = BezierCurve.CubicBezier(curves[0][0], curves[0][1], curves[0][2], curves[0][3], percent);
            var dy = (point - prevPoint).y;
            prevPoint = point;

            base.transform.root.position += base.transform.right * dy;
            prevPosition = base.transform.root.position;
        }


        Vector3[] GenerateCurves()
        {
            List<Vector3> output = new List<Vector3>();
            Vector3[] prev;
            Vector3 point;
            if (curves.Count > 0)
            {
                prev = curves[0];
                output.Add(prev[3]);
                output.Add(2f * prev[3] - prev[2]);
            }
            else
            {
                output.Add(Vector3.zero);
            }

            while (output.Count() < 3)
            {
                point = new Vector3(output[output.Count - 1].x + UnityEngine.Random.Range(5f, 8f), UnityEngine.Random.Range(0.25f, 1.75f) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 0f);
                output.Add(point);
            }

            if (goToZero)
            {
                point = new Vector3(output[output.Count - 1].x + UnityEngine.Random.Range(5f, 8f), 0f, 0f);
                goToZero = false;
            }
            else
            {
                point = new Vector3(output[output.Count - 1].x + UnityEngine.Random.Range(5f, 8f), UnityEngine.Random.Range(0.25f, 1.75f) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 0f);
                goToZero = true;
            }

            output.Add(point);

            if (output[0].x > 0f)
            {
                var x = output[0].x;

                for (int i = 0; i < output.Count; i++)
                {
                    output[i] -= new Vector3(x, 0f, 0f);
                }
            }

            return output.ToArray();
        }
    }
}

namespace WWC.Patches
{
    [HarmonyPatch(typeof(DynamicParticles))]
    class DynamicParticles_Patch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch("PlayBulletHit")]
        private static bool NoSurfaceHitEffect(DynamicParticles __instance, float damage, Transform spawnerTransform, HitInfo hit, Color projectielColor, ref int ___spawnsThisFrame, out int __state)
        {
            __state = ___spawnsThisFrame;
            var shadow = spawnerTransform.gameObject.GetComponent<ShadowMovement_BulletMono>();
            if (shadow)
            {
                ___spawnsThisFrame = 100;
                return false;
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch("PlayBulletHit")]
        private static void NoSurfaceHitEffectTwo(DynamicParticles __instance, float damage, Transform spawnerTransform, HitInfo hit, Color projectielColor, ref int ___spawnsThisFrame, int __state)
        {
            ___spawnsThisFrame = __state;
        }
    }
}