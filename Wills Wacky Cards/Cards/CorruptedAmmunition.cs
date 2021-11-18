using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using WWC.Extensions;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using WillsWackyManagers.Utils;
using WWC.MonoBehaviours;
using Photon.Pun;

namespace WWC.Cards
{
    class CorruptedAmmunition : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            cardInfo.allowMultiple = false;
            ModdingUtils.Extensions.CardInfoExtension.GetAdditionalData(cardInfo).canBeReassigned = false;
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Built");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            WillsWackyCards.instance.ExecuteAfterFrames(20, () => CurseManager.instance.CursePlayer(player, (curse) => { ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, curse); }));
            player.gameObject.GetOrAddComponent<CorruptedAmmunition_Mono>();
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} Added to Player {player.playerID}");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Card] {GetTitle()} removed from Player {player.playerID}");
        }

        protected override string GetTitle()
        {
            return "Corrupted Ammunition";
        }
        protected override string GetDescription()
        {
            return "Fire a number of corrupted bullets equal to curses each battle. An enemy who is hit 5 times by them gains a random curse.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
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
                    stat = "Damage per Curse",
                    amount = "+20%",
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

    public class CorruptionHitEffect : RayHitEffect
    {
        public Player owner;
        public override HasToReturn DoHitEffect(HitInfo hit)
        {
            if (!hit.transform)
            {
                return HasToReturn.canContinue;
            }
            var target = hit.transform.GetComponent<Player>();
            if (target)
            {
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {target.playerID} was hit by a corrupted bullet.");
                if (ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(target.data).isAIMinion)
                {
                    target = ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(target.data).spawner;
                }

                var tracker = owner.GetComponent<CorruptedAmmunition_Mono>();

                if (tracker.corruptionTracker.Keys.Contains(target))
                {
                    tracker.corruptionTracker[target] += 1;
                }
                else
                {
                    tracker.corruptionTracker.Add(target, 1);
                }

                UnityEngine.GameObject.Destroy(this);
            }
            return HasToReturn.canContinue;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }
    }

    [DisallowMultipleComponent]
    class CorruptedAmmunition_Mono : Hooked_Mono
    {
        private int corruptedBullets = 0;
        private GameObject bulletMono = new GameObject("CorruptedBullet", typeof(CorruptionHitEffect));
        public Dictionary<Player, int> corruptionTracker = new Dictionary<Player, int>();
        private bool increased = false;

        private CharacterData data;
        private Player player;
        private Block block;
        private WeaponHandler weaponHandler;
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
                    block = data.block;
                    weaponHandler = data.weaponHandler;
                    gun = weaponHandler.gun;

                    gun.ShootPojectileAction += OnShootProjectileAction;
                }

            }
        }

        private void CheckIfValid()
        {
            var haveCard = false;
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (player.data.currentCards[i].cardName == "Corrupted Ammunition")
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

        private void OnShootProjectileAction(GameObject obj)
        {
            if (corruptedBullets > 0)
            {
                corruptedBullets--;
                ProjectileHit bullet = obj.GetComponent<ProjectileHit>();
                var gameObject = UnityEngine.Object.Instantiate<GameObject>(bulletMono, bullet.transform.position, bullet.transform.rotation, bullet.transform);

                var hitEffect = gameObject.GetComponent<CorruptionHitEffect>();
                hitEffect.owner = player;
            }
            if (corruptedBullets == 0)
            {
                gun.projectileColor = new Color(199f / 255f, 199f / 255f, 0f / 255f, 1f);
            }
        }

        public override void OnGameStart()
        {
            UnityEngine.GameObject.Destroy(this);
        }

        public override void OnPointStart()
        {
            CheckIfValid();
            var tracked = corruptionTracker.Keys.ToArray();
            foreach (var person in tracked)
            {
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {person.playerID} has {corruptionTracker[person]} corruption from player {player.playerID}.");
                while (corruptionTracker[person] > 4)
                {
                    if (this.photonView.IsMine)
                    {
                        this.photonView.RPC(nameof(RPCA_CursePlayer), RpcTarget.All, person.playerID);
                    }
                    corruptionTracker[person] -= 5;
                }
            }

            gun.projectileColor = new Color(58f / 255f, 34f / 255f, 66f / 255f, 1f);
            var curses = CurseManager.instance.GetAllCursesOnPlayer(player);
            corruptedBullets = curses.Count();
            if (!increased)
            {
                gun.damage *= (1f + (corruptedBullets * 0.2f));
                increased = true;
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {player.playerID} has a damage multiplier of {gun.damage} after an increase of {(corruptedBullets * 0.2f) * 100f} from curses."); 
            }
        }

        public override void OnPickStart()
        {
            var tracked = corruptionTracker.Keys.ToArray();
            foreach (var person in tracked)
            {
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {person.playerID} has {corruptionTracker[person]} corruption from player {player.playerID}.");
                while (corruptionTracker[person] > 4)
                {
                    if (this.photonView.IsMine)
                    {
                        this.photonView.RPC(nameof(RPCA_CursePlayer), RpcTarget.All, person.playerID);
                    }
                    corruptionTracker[person] -= 5;
                }
            }
        }

        public override void OnPointEnd()
        {
            if (increased)
            {
                increased = false;
                gun.damage /= (1f + (corruptedBullets * 0.2f));
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {player.playerID} has a damage multiplier of {gun.damage} after an decrease of {(corruptedBullets * 0.2f) * 100f} from curses."); 
            }
        }

        [PunRPC]
        private void RPCA_CursePlayer(int id)
        {
            CurseManager.instance.CursePlayer(PlayerManager.instance.GetPlayerWithID(id));
        }

        private void OnDestroy()
        {
            if (increased)
            {
                increased = false;
                gun.damage /= (1f + (corruptedBullets * 0.2f));
                UnityEngine.Debug.Log($"[{WillsWackyCards.ModInitials}][Corrupted Ammunition][Debugging] Player {player.playerID} has a damage multiplier of {gun.damage} after an decrease of {(corruptedBullets * 0.2f) * 100f} from curses.");
            }
            gun.ShootPojectileAction -= OnShootProjectileAction;
        }
    }
}
